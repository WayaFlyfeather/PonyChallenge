using PonyChallenge.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PonyChallenge
{
    public partial class MazeNavigator
    {
        readonly MazeSnapshot maze;

        public MazeNavigator(MazeSnapshot maze)
        {
            this.maze = maze;
        }


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
