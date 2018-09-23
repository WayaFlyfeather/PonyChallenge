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
        public MazePoint FromDirection(byte direction)
        {
            switch (direction)
            {
                case 0: return NorthOf; 
                case 1: return EastOf;
                case 2: return SouthOf;
                case 3: return WestOf;
                default: throw new ApplicationException("Non-existant direction");
            }
        }
    }
}
