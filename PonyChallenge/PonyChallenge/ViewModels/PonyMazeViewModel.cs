using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
                if (model.PlayerName!=value)
                {
                    model.PlayerName = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsValid));
                    createMazeCommand?.ChangeCanExecute();
                }
            }
        }

        public List<int> Difficulties { get; } = new List<int> { 0, 1, 2, 3 };
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
                if (model.Width!=value)
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

//                Maze newMaze = await ((App)App.Current).PonyMazeService.CreateMaze(model.Width, model.Height, model.PlayerName, model.Difficulty);
                Maze newMaze = new Maze() { Width = Model.Width, Height = Model.Height, PlayerName = Model.PlayerName, Difficulty = Model.Difficulty, Id = "df5392d2-3d35-4287-9ffd-5d20e59f3f11" };
//                Debug.WriteLine("Maze created, id: " + newMaze.Id);
                newMaze.Positions = await ((App)App.Current).PonyMazeService.GetSnapshot(newMaze.Id);
                Debug.WriteLine("Got positions for Maze " + newMaze.Id);
                Model = newMaze;
                createMazeCommand.ChangeCanExecute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in CreateMaze: " + ex.Message);
            }
        }

        private Command createMazeCommand;
        public System.Windows.Input.ICommand CreateMazeCommand => createMazeCommand ?? (createMazeCommand = new Command(createMaze_Execute, createMaze_CanExecute));

    }
}
