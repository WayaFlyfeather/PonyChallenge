using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge.Models
{
    public class MazeLocation
    {
        public BitArray Walls { get; set; } = new BitArray(4, false);
        public bool NorthWall
        {
            get => Walls[0];
            set { Walls[0] = value; }
        }
        public bool EastWall
        {
            get => Walls[1];
            set { Walls[1] = value; }
        }
        public bool SouthWall
        {
            get => Walls[2];
            set { Walls[2] = value; }
        }
        public bool WestWall
        {
            get => Walls[3];
            set { Walls[3] = value; }
        }

        public bool ContainsDomokun { get; set; } = false;
        public bool ContainsPony { get; set; } = false;
        public bool IsExit { get; set; } = false;
    }
}
