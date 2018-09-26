using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge
{
    public class MazeNavigator
    {
        readonly MazeSnapshot maze;

        public MazeNavigator(MazeSnapshot maze)
        {
            this.maze = maze;
        }

        /// <summary>
        /// The algorithm used for finding the best way to move is as follows
        /// 
        /// 1. Rule out moving into walls
        /// 2. Rule out moving into domokun
        /// 3. If the exit is in one of the spaces, go there (to win)
        /// 4. Rule out moving into a space one space from domokun
        /// 5. If there by now is nowhere to go, go to one of the places ruled out last (most likely to be caught)
        /// 6. Of the remaining directions, find the one leading to the exit in the fewest steps.
        /// 7. If none does, find the shortest way to a loop in the maze, that is, one with a path connecting back
        /// 8. If none does, find the one with the most steps to a cul-de-sac. If the domokun is a bit smart, it will
        ///    follow, and the pony will be caught, but it will get a run for it's money, at least.
        ///    
        /// Note: step 6 is partly wasted, as in the mazes served there are only one way to the exit, anyway.
        ///       And step 7 is completely wasted, as there are no loops in the mazes. It would have been a nice way to
        ///       outsmart the domokun, though.
        ///       Step 6, 7 and 8 are accomplished using recursive method calls.
        ///       
        ///       When I originally considered the algorithm to use, I had no idea what the mazes would be like.
        ///       And I imagined them being much more open, with more possibilities, at least in some difficulties.
        ///       Therefore I also had ideas about deciding whether moving in one direction would be smart, based on whether
        ///       the domokun could intercept, or another, more roundabout way should be taken. All in all I was somewhat 
        ///       disappointed in the very limiting design of the mazes.
        /// </summary>
        /// <returns>The best direction for a pony to move</returns>
        public byte FindDirection()
        {
            byte?[] ruledOutDirections = new byte?[4];
            MazeLocation thisLocation = maze.Locations[maze.PonyPlacement.X, maze.PonyPlacement.Y];
            for (byte direction = 0; direction < 4; direction++)
                ruledOutDirections[direction] = thisLocation.Walls[direction] ? (byte?)0 : (byte?)null;

            byte? onlyWay = checkForOnlyWay(ruledOutDirections);
            if (onlyWay.HasValue)
                return onlyWay.Value;

            byte? domokunOneStepAway = checkDomokunInNeighboringLocation(maze.PonyPlacement);
            if (domokunOneStepAway.HasValue)
            {
                ruledOutDirections[domokunOneStepAway.Value] = 1;
                onlyWay = checkForOnlyWay(ruledOutDirections);
                if (onlyWay.HasValue)
                    return onlyWay.Value;
            }

            for (byte direction = 0; direction < 4; direction++)
            {
                if (ruledOutDirections[direction].HasValue) //skip if already ruled out
                    continue;

                MazePoint neighborPoint = maze.PonyPlacement.FromDirection(direction);
                MazeLocation neighborLocation = maze.Locations[neighborPoint.X, neighborPoint.Y];
                if (neighborLocation.IsExit) //the exit is just one step away, go there!
                    return direction;
            }

            if (!domokunOneStepAway.HasValue)
            {
                for (byte direction = 0; direction < 4; direction++)
                {
                    if (ruledOutDirections[direction].HasValue) //skip if already ruled out
                        continue;

                    byte? domokunTwoStepsAway = checkDomokunInNeighboringLocation(maze.PonyPlacement.FromDirection(direction));
                    if (domokunTwoStepsAway.HasValue)
                        ruledOutDirections[direction] = 2;
                }
            }

            onlyWay = checkForOnlyWay(ruledOutDirections);
            if (onlyWay.HasValue)
                return onlyWay.Value;

            int?[] directionResults = new int?[4];
            StepTracker tracker = new StepTracker(maze.Locations.GetLength(0), maze.Locations.GetLength(1));
            tracker.MarkStep(maze.PonyPlacement);

            for (byte direction = 0; direction < 4; direction++)
            {
                if (ruledOutDirections[direction].HasValue)
                    directionResults[direction] = null;
                else
                {
                    directionResults[direction] = stepsToExit(maze.PonyPlacement.FromDirection(direction), 1, tracker, false);
                }
            }
            byte? bestDirection = getShortestRoute(directionResults);
            if (bestDirection.HasValue)
                return bestDirection.Value;

            for (byte direction = 0; direction < 4; direction++)
            {
                if (ruledOutDirections[direction].HasValue)
                    directionResults[direction] = null;
                else
                    directionResults[direction] = stepsToLoop(direction, maze.PonyPlacement.FromDirection(direction), 1, tracker);
            }
            bestDirection = getShortestRoute(directionResults);
            if (bestDirection.HasValue)
                return bestDirection.Value;

            for (byte direction = 0; direction < 4; direction++)
            {
                if (ruledOutDirections[direction].HasValue)
                    directionResults[direction] = null;
                else
                {
                    directionResults[direction] = countLongestPath(direction, maze.PonyPlacement.FromDirection(direction), 1, tracker);
                    if (!directionResults[direction].HasValue)
                        ruledOutDirections[direction] = 3;
                }
            }

            bestDirection = getLongestRoute(directionResults);
            if (bestDirection.HasValue)
                return bestDirection.Value;

            onlyWay = checkForOnlyWay(ruledOutDirections);
            if (onlyWay.HasValue)
                return onlyWay.Value;
            else
                throw new ApplicationException("Can't find a move - shouldn't happen");
        }

        int? stepsToExit(MazePoint position, int stepCount, StepTracker tracks, bool safeRouteOnly)
        {
            if (tracks.HasStepped(position))
                return null;

            MazeLocation thisLocation = maze.Locations[position.X, position.Y];
            if (thisLocation.IsExit)
                return stepCount;

            if (thisLocation.ContainsDomokun && safeRouteOnly)
                return null;

            int? bestResult = null;
            StepTracker newTracks = tracks.CloneWithStep(position);

            for (byte direction = 0; direction < 4; direction++)
            {
                if (!thisLocation.Walls[direction])
                {
                    int? directionResult = stepsToExit(position.FromDirection(direction), stepCount + 1, newTracks, safeRouteOnly);
                    if (!bestResult.HasValue || (directionResult.HasValue && directionResult.Value < bestResult.Value))
                        bestResult = directionResult;
                }
            }

            return bestResult;
        }

        int? countLongestPath(byte entryDirection, MazePoint position, int stepCount, StepTracker tracks)
        {
            if (tracks.HasStepped(position))
                return null;

            MazeLocation thisLocation = maze.Locations[position.X, position.Y];
            if (thisLocation.ContainsDomokun)
                return null;

            int? bestResult = null;
            StepTracker newTracks = tracks.CloneWithStep(position);

            bool culDeSac = true;
            for (byte direction = 0; direction < 4; direction++)
            {
                if ((entryDirection == 0 && direction == 2) || (entryDirection == 2 && direction == 0) || (entryDirection == 3 && direction == 1) || (entryDirection == 1 && direction == 3))
                    continue;

                if (!thisLocation.Walls[direction])
                {
                    culDeSac = false;
                    int? result = countLongestPath(direction, position.FromDirection(direction), stepCount + 1, newTracks);
                    if (!bestResult.HasValue || result.HasValue && result.Value > bestResult.Value)
                        bestResult = result;
                }
            }
            if (culDeSac)
                return stepCount;

            return bestResult;
        }

        int? stepsToLoop(byte entryDirection, MazePoint position, int stepCount, StepTracker tracks)
        {
            if (tracks.HasStepped(position))
                return stepCount;

            MazeLocation thisLocation = maze.Locations[position.X, position.Y];

            if (thisLocation.ContainsDomokun)
                return null;

            int? bestResult = null;
            StepTracker newTracks = tracks.CloneWithStep(position);

            for (byte direction = 0; direction < 4; direction++)
            {
                if ((entryDirection == 0 && direction == 2) || (entryDirection == 2 && direction == 0) || (entryDirection == 3 && direction == 1) || (entryDirection == 1 && direction == 3))
                    continue;

                if (!thisLocation.Walls[direction])
                {
                    int? result = stepsToLoop(direction, position.FromDirection(direction), stepCount + 1, newTracks);
                    if (!bestResult.HasValue || result.HasValue && result.Value < bestResult.Value)
                        bestResult = result;
                }
            }

            return bestResult;
        }

        byte? checkDomokunInNeighboringLocation(MazePoint point)
        {
            MazeLocation location = maze.Locations[point.X, point.Y];
            for (byte direction = 0; direction < 4; direction++)
            {
                if (location.Walls[direction])
                    continue;

                MazePoint neighborPoint = point.FromDirection(direction);
                MazeLocation neighborLocation = maze.Locations[neighborPoint.X, neighborPoint.Y];
                if (neighborLocation.ContainsDomokun)
                    return direction;
            }

            return null;
        }

        byte? getShortestRoute(int?[] routeResults)
        {
            byte bestRoute = 0;
            for (byte direction = 1; direction < 4; direction++)
            {
                if (!routeResults[bestRoute].HasValue || (routeResults[direction].HasValue && routeResults[direction] < routeResults[bestRoute]))
                    bestRoute = direction;
            }

            if (routeResults[bestRoute].HasValue)
                return bestRoute;
            else
                return null;
        }

        byte? getLongestRoute(int?[] routeResults)
        {
            byte bestRoute = 0;
            for (byte direction = 1; direction < 4; direction++)
            {
                if (!routeResults[bestRoute].HasValue || (routeResults[direction].HasValue && routeResults[direction] > routeResults[bestRoute]))
                    bestRoute = direction;
            }

            if (routeResults[bestRoute].HasValue)
                return bestRoute;
            else
                return null;
        }

        byte? checkForOnlyWay(byte?[] ruledOutDirections)
        {
            byte? forcedDirection = null;

            int highRuleoutValue = -1;
            byte highRuleoutDirection = 0;
            for (byte direction = 0; direction < 4; direction++)
            {
                if (!ruledOutDirections[direction].HasValue)
                {
                    if (forcedDirection.HasValue)
                        return null; // at least two possible routes; no forced move.
                    else
                        forcedDirection = direction;
                }
                else
                {
                    if (ruledOutDirections[direction].Value > highRuleoutValue)
                    {
                        highRuleoutValue = ruledOutDirections[direction].Value;
                        highRuleoutDirection = direction;
                    }
                }
            }

            if (!forcedDirection.HasValue) //no possible moves found; use the one ruled out last.
                return highRuleoutDirection;

            return forcedDirection;
        }
    }
}
