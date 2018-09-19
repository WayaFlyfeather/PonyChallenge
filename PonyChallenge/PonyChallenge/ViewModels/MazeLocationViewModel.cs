using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge.ViewModels
{
    public class MazeLocationViewModel : BindableBase
    {
        private MazeLocation model;
        public MazeLocation Model
        {
            get => model;
            set => SetProperty(ref model, value);
        }

        public MazeLocationViewModel(MazeLocation model)
        {
            this.model = model;
        }

        public bool NorthWall => model.NorthWall;
        public bool EastWall => model.EastWall;
        public bool SouthWall => model.SouthWall;
        public bool WestWall => model.WestWall;

        public bool NorthWestCorner => model.WestWall || model.NorthWall;
        public bool NorthEastCorner => model.EastWall || model.NorthWall;
        public bool SouthWestCorner => model.WestWall || model.SouthWall;
        public bool SouthEastCorner => model.EastWall || model.SouthWall;

        public bool IsExit => model.IsExit;
        public bool ContainsDomokun => model.ContainsDomokun;
        public bool ContainsPony => model.ContainsPony;
    }
}
