using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PonyChallenge.Services
{
    interface IPonyMazeService
    {
        Task<Maze> CreateMaze(Maze maze);
    }
}
