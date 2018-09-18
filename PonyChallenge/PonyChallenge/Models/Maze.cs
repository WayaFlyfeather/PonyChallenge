using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge.Models
{
    public class Maze
    {
        public string Id { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public string PlayerName { get; set; }

        public int Difficulty { get; set; }

        public MazeSnapshot Positions { get; set; } = null;
    }
}
