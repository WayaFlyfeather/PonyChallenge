using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge.Models
{
    public struct MazePoint
    {
        public byte X;
        public byte Y;

        public MazePoint NorthOf => new MazePoint() { X = this.X, Y = (byte)(this.Y - 1) };
        public MazePoint SouthOf => new MazePoint() { X = this.X, Y = (byte)(this.Y + 1) };
        public MazePoint WestOf => new MazePoint() { X = (byte)(this.X - 1), Y = this.Y };
        public MazePoint EastOf => new MazePoint() { X = (byte)(this.X + 1), Y = this.Y };
    }
}
