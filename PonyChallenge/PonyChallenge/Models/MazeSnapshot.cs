namespace PonyChallenge.Models
{
    public enum MazeState { Active, Won, Lost };

    public class MazeSnapshot
    {
        public MazeLocation[,] Locations { get; set; }
        public MazePoint DomokunPlacement { get; set; }
        public MazePoint PonyPlacement { get; set; }
        public MazePoint Exit { get; set; }
        public MazeState State { get; set; }
    }
}
