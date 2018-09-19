using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge.Models
{
    public class Maze
    {
        public string Id { get; set; } = null;

        public int Width { get; set; } = 18;
        public int Height { get; set; } = 22;

        public string PlayerName { get; set; } = "Rarity";

        public int Difficulty { get; set; } = 0;

        public MazeSnapshot Positions { get; set; } = null;
    }
}
