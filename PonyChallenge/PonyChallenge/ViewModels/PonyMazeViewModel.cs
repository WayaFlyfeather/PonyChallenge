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

        public List<string> PonyNames { get; } = new List<string> { "Rarity", "Applejack", "Spike" };

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

        public List<int> Difficulties { get; } = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        public int SelectedDifficulty
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

        public MazeSnapshot LatestSnapshot
        {
            get => model.Positions;
            set
            {
                if (model.Positions != value)
                {
                    model.Positions = value;                    
                    OnPropertyChanged();
                    BestNextMove = null;
                    moveDirectionCommand.ChangeCanExecute();
                    if (model.Positions!=null && model.Positions.State == MazeState.Active)
                    {
                        Task.Run(() =>
                        {
                            Debug.WriteLine("Starting to find next move");
                            int? nextMove = findDirection();
                            Device.BeginInvokeOnMainThread(() => { BestNextMove = nextMove; });
                            Debug.WriteLine("Next move determined");
                        });
                    }
                }
            }
        }

        public string Specs => String.Format($"{Width},{Height} {SelectedPonyName} ({SelectedDifficulty})");

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(model.PlayerName) || !PonyNames.Contains(model.PlayerName))
                    return false;
                if (!Difficulties.Contains(model.Difficulty))
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
            return IsValid && model.Id == null;
        }

        async void createMaze_Execute()
        {
            try
            {

                Model = await ((App)App.Current).PonyMazeService.CreateMaze(model.Width, model.Height, model.PlayerName, model.Difficulty);
                Debug.WriteLine("Maze created, id: " + Model.Id);
                createMazeCommand.ChangeCanExecute();
                LatestSnapshot = await ((App)App.Current).PonyMazeService.GetSnapshot(Model.Id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in CreateMaze: " + ex.Message);
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
        TimeSpan minDelay = TimeSpan.FromMilliseconds(1000);

        bool timerStop = false;

        public void StartTick()
        {
            timerStop = false;
            Device.StartTimer(TimeSpan.FromMilliseconds(200), tick);
        }

        public void StopTick()
        {
            timerStop = true;
        }

        bool tick()
        {
            if (timerStop)
                return false;

            try
            {
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
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in timer " + ex.Message);
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
            await ((App)App.Current).PonyMazeService.Move(Model.Id, directionNames[direction]);
            LatestSnapshot = await ((App)App.Current).PonyMazeService.GetSnapshot(Model.Id);
            if (LatestSnapshot.State != MazeState.Active)
                MakeRepeatingAutoMoves = false;
            lastMove = DateTimeOffset.Now;
        }

        bool canPonyMoveInDirection(int direction)
        {
            if (LatestSnapshot.State != MazeState.Active)
                return false;

            MazeLocation ponyLocation = LatestSnapshot?.Locations[LatestSnapshot.PonyPlacement.X, LatestSnapshot.PonyPlacement.Y];
            if (ponyLocation is null)
                return false;

            return !ponyLocation.Walls[direction];
        }

        //async Task makeMockMove(int direction)
        //{
        //    model.Positions.Locations[model.Positions.PonyPlacement.X, model.Positions.PonyPlacement.Y].ContainsPony = false;
        //    switch (direction)
        //    {
        //        case 0: model.Positions.PonyPlacement = model.Positions.PonyPlacement.NorthOf; break;
        //        case 1: model.Positions.PonyPlacement = model.Positions.PonyPlacement.EastOf; break;
        //        case 2: model.Positions.PonyPlacement = model.Positions.PonyPlacement.SouthOf; break;
        //        case 3: model.Positions.PonyPlacement = model.Positions.PonyPlacement.WestOf; break;
        //        default: break;
        //    }
        //    model.Positions.Locations[model.Positions.PonyPlacement.X, model.Positions.PonyPlacement.Y].ContainsPony = true;
        //    MazeSnapshot temp = LatestSnapshot;
        //    LatestSnapshot = null;
        //    await Task.Delay(5);
        //    LatestSnapshot = temp;
        //}

        class StepTracker
        {
            BitArray[] rows;

            public StepTracker(int width, int height)
            {
                rows = new BitArray[height];
                for (int r = 0; r < height; r++)
                    rows[r] = new BitArray(width, false);
            }

            StepTracker(StepTracker other)
            {
                rows = new BitArray[other.rows.Length];
                for (int r = 0; r < other.rows.Length; r++)
                    rows[r] = (BitArray)other.rows[r].Clone();
            }

            public bool HasStepped(MazePoint position)
            {
                return rows[position.Y][position.X];
            }

            public StepTracker Clone()
            {
                return new StepTracker(this);
            }

            public void MarkStep(MazePoint position)
            {
                rows[position.Y][position.X] = true;
            }
        }
    
        int? stepsToExit(MazePoint position, int stepCount, StepTracker tracks, bool safeRouteOnly)
        {
            if (tracks.HasStepped(position))
                return null;

            MazeLocation thisLocation = model.Positions.Locations[position.X, position.Y];
            if (thisLocation.IsExit)
                return stepCount;

            if (thisLocation.ContainsDomokun && safeRouteOnly)
                return null;

            int? bestResult = null;

            StepTracker newTracks = tracks.Clone();

            newTracks.MarkStep(position);

            if (!thisLocation.NorthWall)
                bestResult = stepsToExit(position.NorthOf, stepCount + 1, newTracks, safeRouteOnly);

            if (!thisLocation.EastWall)
            {
                int? eastResult= stepsToExit(position.EastOf, stepCount + 1, newTracks, safeRouteOnly);
                if (!bestResult.HasValue || (eastResult.HasValue && eastResult.Value < bestResult.Value))
                    bestResult = eastResult;
            }

            if (!thisLocation.SouthWall)
            {
                int? southResult = stepsToExit(position.SouthOf, stepCount + 1, newTracks, safeRouteOnly);
                if (!bestResult.HasValue || (southResult.HasValue && southResult.Value < bestResult.Value))
                    bestResult = southResult;
            }

            if (!thisLocation.WestWall)
            {
                int? westResult = stepsToExit(position.WestOf, stepCount + 1, newTracks, safeRouteOnly);
                if (!bestResult.HasValue || (westResult.HasValue && westResult.Value < bestResult.Value))
                    bestResult = westResult;
            }

            return bestResult;
        }

        byte? checkDomokunInNeighboringLocation(MazePoint point)
        {
            MazeLocation location = model.Positions.Locations[point.X, point.Y];
            for (byte direction = 0; direction < 4; direction++)
            {
                if (location.Walls[direction]) 
                    continue;

                MazePoint neighborPoint = point.FromDirection(direction);
                MazeLocation neighborLocation = Model.Positions.Locations[neighborPoint.X, neighborPoint.Y];
                if (neighborLocation.ContainsDomokun)
                    return direction;
            }

            return null;
        }

        byte findDirection()
        {
            byte?[] ruledOutDirections = new byte?[4];
            MazeLocation thisLocation = model.Positions.Locations[model.Positions.PonyPlacement.X, model.Positions.PonyPlacement.Y];
            for (byte direction = 0; direction < 4; direction++)
                ruledOutDirections[direction] = thisLocation.Walls[direction] ? (byte?)0 : (byte?)null;

            byte? onlyWay = checkForOnlyWay(ruledOutDirections);
            if (onlyWay.HasValue)
                return onlyWay.Value;

            byte? domokunOneStepAway = checkDomokunInNeighboringLocation(model.Positions.PonyPlacement);
            if (domokunOneStepAway.HasValue)
            {
                ruledOutDirections[domokunOneStepAway.Value] = 1;
                onlyWay = checkForOnlyWay(ruledOutDirections);
                if (onlyWay.HasValue)
                    return onlyWay.Value;
            }

            for (byte direction = 0; direction < 4; direction++)
            {
                if (ruledOutDirections[direction].HasValue) //skip if already ruled out
                    continue;

                MazePoint neighborPoint = Model.Positions.PonyPlacement.FromDirection(direction);
                MazeLocation neighborLocation = Model.Positions.Locations[neighborPoint.X, neighborPoint.Y];
                if (neighborLocation.IsExit) //the exit is just one step away, go there!
                    return direction;
            }

            if (!domokunOneStepAway.HasValue)
            {
                for (byte direction = 0; direction < 4; direction++)
                {
                    if (ruledOutDirections[direction].HasValue) //skip if already ruled out
                        continue;

                    byte? domokunTwoStepsAway = checkDomokunInNeighboringLocation(Model.Positions.PonyPlacement.FromDirection(direction));
                    if (domokunTwoStepsAway.HasValue)
                        ruledOutDirections[direction] = 2;
                }
            }

            onlyWay = checkForOnlyWay(ruledOutDirections);
            if (onlyWay.HasValue)
                return onlyWay.Value;

            int?[] directionResults = new int?[4];
            StepTracker tracker = new StepTracker(model.Width, model.Height);
            tracker.MarkStep(model.Positions.PonyPlacement);

            for (byte direction = 0; direction < 4; direction++)
            {
                if (ruledOutDirections[direction].HasValue)
                    directionResults[direction] = null;
                else
                {
                    directionResults[direction] = stepsToExit(model.Positions.PonyPlacement.FromDirection(direction), 1, tracker, false);
                    if (!directionResults[direction].HasValue)
                        ruledOutDirections[direction] = 3;
                }
            }

            byte bestDirection = 0;

            for (byte direction=1; direction < 4; direction++)
            {
                if (!directionResults[bestDirection].HasValue || (directionResults[direction].HasValue && directionResults[direction]<directionResults[bestDirection]))
                    bestDirection = direction;
            }

            if (directionResults[bestDirection].HasValue)
                return bestDirection;

            onlyWay = checkForOnlyWay(ruledOutDirections);
            if (onlyWay.HasValue)
                return onlyWay.Value;
            else
                throw new ApplicationException("Can't find a move - shouldn't happen");
        }

        byte? checkForOnlyWay(byte?[] ruledOutDirections)
        {
            byte? forcedDirection = null;

            int highRuleoutValue = -1;
            byte highRuleoutDirection = 0;
            for (byte direction=0; direction<4; direction++)
            {
                if (!ruledOutDirections[direction].HasValue)
                {
                    if (forcedDirection.HasValue)
                        return null; // at least two possible routes; no forced move.
                    else
                        forcedDirection = direction;
                }
                else
                {
                    if (ruledOutDirections[direction].Value > highRuleoutValue)
                    {
                        highRuleoutValue = ruledOutDirections[direction].Value;
                        highRuleoutDirection = direction;
                    }
                }
            }

            if (!forcedDirection.HasValue) //no possible moves found; use the one ruled out last.
                return highRuleoutDirection;

            return forcedDirection;
        }            
    }
}
