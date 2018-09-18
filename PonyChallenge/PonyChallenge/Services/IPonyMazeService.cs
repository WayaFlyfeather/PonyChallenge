using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PonyChallenge.Services
{
    interface IPonyMazeService
    {
        Task<Maze> CreateMaze(int width, int height, string playerName, int difficulty);
        Task<MazeSnapshot> GetSnapshot(string mazeId);
        Task Move(string mazeId, string direction);
    }
}
