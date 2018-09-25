using PonyChallenge.Models;
using System.Collections;

namespace PonyChallenge
{
    public partial class MazeNavigator
    {
        public class StepTracker
        {
            BitArray[] rows;

            public StepTracker(int width, int height)
            {
                rows = new BitArray[height];
                for (int r = 0; r < height; r++)
                    rows[r] = new BitArray(width, false);
            }

            StepTracker(StepTracker other)
            {
                rows = new BitArray[other.rows.Length];
                for (int r = 0; r < other.rows.Length; r++)
                    rows[r] = (BitArray)other.rows[r].Clone();
            }

            public bool HasStepped(MazePoint position)
            {
                return rows[position.Y][position.X];
            }

            public StepTracker CloneWithStep(MazePoint stepPosition)
            {
                StepTracker clone = new StepTracker(this);
                clone.MarkStep(stepPosition);

                return clone;
            }

            public void MarkStep(MazePoint stepPosition)
            {
                this.rows[stepPosition.Y][stepPosition.X] = true;
            }
        }
    }
}
