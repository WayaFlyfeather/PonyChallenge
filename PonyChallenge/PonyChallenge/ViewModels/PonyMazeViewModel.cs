using PonyChallenge.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PonyChallenge.ViewModels
{
    public class PonyMazeViewModel : BindableBase
    {
        private Maze model;
        public Maze Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }

        public PonyMazeViewModel()
        {
            model = new Maze();
        }

        public List<string> PonyNames { get; } = new List<string> { "Rarity", "Applejack", "Pinkie Pie", "Spike" };

        public string SelectedPonyName
        {
            get => model.PlayerName;
            set
            {
                if (model.PlayerName != value)
                {
                    model.PlayerName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsValid));
                    createMazeCommand?.ChangeCanExecute();
                }
            }
        }

        public int Difficulty
        {
            get => model.Difficulty;
            set
            {
                if (model.Difficulty != value)
                {
                    model.Difficulty = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsValid));
                    createMazeCommand?.ChangeCanExecute();
                }
            }
        }


        public int Width
        {
            get => model.Width;
            set
            {
                if (model.Width != value)
                {
                    model.Width = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsValid));
                    createMazeCommand?.ChangeCanExecute();
                }
            }
        }

        public int Height
        {
            get => model.Height;
            set
            {
                if (model.Height != value)
                {
                    model.Height = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsValid));
                    createMazeCommand?.ChangeCanExecute();
                }
            }
        }


        private int movesPerSecondPower = 1;
        public int MovesPerSecondPower
        {
            get => movesPerSecondPower;
            set
            {
                if (SetProperty(ref movesPerSecondPower, value))
                    minDelay = TimeSpan.FromMilliseconds(1000 / Math.Pow(2, value));
            }
        }

        public MazeSnapshot LatestSnapshot
        {
            get => model.Positions;
            set
            {
                if (model.Positions != value)
                {
                    bool nullMaze = (model.Positions is null) || (value is null);
                    model.Positions = value;
                    OnPropertyChanged();
                    if (nullMaze)
                    {
                        OnPropertyChanged(nameof(HasSnapshot));
                        OnPropertyChanged(nameof(IsValid));
                        createMazeCommand?.ChangeCanExecute();
                    }
                    BestNextMove = null;
                    moveDirectionCommand.ChangeCanExecute();
                    if (model.Positions != null && model.Positions.State == MazeState.Active)
                    {
                        Task.Run(() =>
                        {
                            MazeNavigator navigator = new MazeNavigator(model.Positions);
                            int? nextMove = navigator.FindDirection();
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                BestNextMove = nextMove;
                            });
                        });
                    }
                }
            }
        }


        private bool unreportedConnectionError = false;
        public bool UnreportedConnectionError
        {
            get => unreportedConnectionError;
            set => SetProperty(ref unreportedConnectionError, value);
        }

        public bool HasSnapshot => !(LatestSnapshot is null);

        public string Specs => String.Format($"{Width},{Height} {SelectedPonyName} ({Difficulty})");

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(model.PlayerName) || !PonyNames.Contains(model.PlayerName))
                    return false;
                if (model.Difficulty < 0 || model.Difficulty > 10)
                    return false;
                if (model.Width < 15 || model.Width > 25)
                    return false;
                if (model.Height < 15 || model.Height > 25)
                    return false;

                return true;
            }
        }

        bool createMaze_CanExecute()
        {
            return IsValid && String.IsNullOrEmpty(model.Id);
        }

        public void ResetMaze()
        {
            Model.Id = null;
            LatestSnapshot = null;
        }

        async void createMaze_Execute()
        {
            try
            {

                Model = await ((App)App.Current).PonyMazeService.CreateMaze(model.Width, model.Height, model.PlayerName, model.Difficulty);
                createMazeCommand.ChangeCanExecute();
                LatestSnapshot = await ((App)App.Current).PonyMazeService.GetSnapshot(Model.Id);
            }
            catch (Exception ex)
            {
                ResetMaze();
                Debug.WriteLine("Exception in CreateMaze: " + ex.Message);
                UnreportedConnectionError = true;
            }
        }


        private bool makeRepeatingAutoMoves;
        public bool MakeRepeatingAutoMoves
        {
            get => makeRepeatingAutoMoves;
            set => SetProperty(ref makeRepeatingAutoMoves, value);
        }

        private int? bestNextMove;
        public int? BestNextMove
        {
            get => bestNextMove;
            set
            {
                if (SetProperty(ref bestNextMove, value))
                {
                    OnPropertyChanged(nameof(HasBestNextMove));
                    makeAutoMoveCommand?.ChangeCanExecute();
                }
            }
        }

        public bool HasBestNextMove => bestNextMove.HasValue;

        private Command createMazeCommand;
        public ICommand CreateMazeCommand => createMazeCommand ?? (createMazeCommand = new Command(createMaze_Execute, createMaze_CanExecute));

        private Command makeAutoMoveCommand;
        public ICommand MakeAutoMoveCommand => makeAutoMoveCommand ?? (makeAutoMoveCommand = new Command(makeAutoMove_Execute, makeAutoMove_CanExecute));

        private Command switchRepeatAutoMoveCommand;
        public ICommand SwitchRepeatAutoMoveCommand => switchRepeatAutoMoveCommand ?? (switchRepeatAutoMoveCommand = new Command(() => MakeRepeatingAutoMoves = !MakeRepeatingAutoMoves));

        private Command<string> moveDirectionCommand;
        public ICommand MoveDirectionCommand => moveDirectionCommand ?? (moveDirectionCommand = new Command<string>(async (directionString) => { MakeRepeatingAutoMoves = false; await makeMove(int.Parse(directionString)); }, (directionString) => canPonyMoveInDirection(int.Parse(directionString))));

        bool makeAutoMove_CanExecute()
        {
            return HasBestNextMove;
        }

        DateTimeOffset lastMove = DateTimeOffset.Now;
        TimeSpan minDelay = TimeSpan.FromMilliseconds(500);

        bool timerStop = false;

        public void StartTick()
        {
            timerStop = false;
            Device.StartTimer(TimeSpan.FromMilliseconds(50), tick);
        }

        public void StopTick()
        {
            timerStop = true;
        }

        bool tick()
        {
            if (timerStop)
                return false;

            if (MakeRepeatingAutoMoves)
            {
                if (BestNextMove.HasValue)
                {
                    if (DateTimeOffset.Now > lastMove + minDelay)
                    {
                        lastMove = DateTimeOffset.Now;
                        Device.BeginInvokeOnMainThread(async () => { if (BestNextMove.HasValue) await makeMove(BestNextMove.Value); });
                    }
                }
            }

            return !timerStop;
        }

        async void makeAutoMove_Execute()
        {
            MakeRepeatingAutoMoves = false;
            if (!BestNextMove.HasValue)
                return;

            await makeMove(BestNextMove.Value);
        }

        readonly string[] directionNames = { "north", "east", "south", "west" };

        async Task makeMove(int direction)
        {
            if (!canPonyMoveInDirection(direction))
                return;

            try
            {
                await ((App)App.Current).PonyMazeService.Move(Model.Id, directionNames[direction]);
                MazeSnapshot snap = await ((App)App.Current).PonyMazeService.GetSnapshot(Model.Id);
                if (snap.State != MazeState.Active)
                    MakeRepeatingAutoMoves = false;
                lastMove = DateTimeOffset.Now;
                LatestSnapshot = snap;
            }
            catch(Exception ex)
            {
                MakeRepeatingAutoMoves=false;
                Debug.WriteLine("Exception in makeMove: " + ex.Message);
                UnreportedConnectionError = true;
            }
        }

        bool canPonyMoveInDirection(int direction)
        {
            if (LatestSnapshot is null)
                return false;

            if (LatestSnapshot.State != MazeState.Active)
                return false;

            MazeLocation ponyLocation = LatestSnapshot?.Locations[LatestSnapshot.PonyPlacement.X, LatestSnapshot.PonyPlacement.Y];
            if (ponyLocation is null)
                return false;

            return !ponyLocation.Walls[direction];
        }
    }
}
