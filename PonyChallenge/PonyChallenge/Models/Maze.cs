using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge.Models
{
    public class Maze
    {
        [JsonProperty("maze-id")]
        public string MazeId { get; set; }

        [JsonProperty("maze-width")]
        public int MazeWidth { get; set; }

        [JsonProperty("maze-height")]
        public int MazeHeight { get; set; }

        [JsonProperty("maze-player-name")]
        public string MazePlayerName { get; set; }

        [JsonProperty("difficulty")]
        public int Difficulty { get; set; }
    }
}
