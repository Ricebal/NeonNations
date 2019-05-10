using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Soldier.Bot.DStar
{
    public static class BresenhamPathSmoothing
    {
        public static Coordinates FarthestNodeToReach(Coordinates currentNode, List<Node> nodesToTraverse, Node[][] map)
        {
            int nextNode = 0;
            int previousNextNode = nextNode;
            int goalNode = nodesToTraverse.Count - 1;

            // When it's possible to move from current position and the new position AND the new position is not the final destination
            while (PossibleToMoveBetween(currentNode, nodesToTraverse[nextNode], map) && nextNode != goalNode)
            {
                // Save current new point for if the next one fails.
                previousNextNode = nextNode;
                // Get new point that is closer to the destination.
                nextNode++;
            }

            // If the newPoint is not the destination.
            if (nextNode != goalNode)
            {
                // Get back to previousPoint, since that's the last point that is reachable.
                nextNode = previousNextNode;
            }
            return new Coordinates(nodesToTraverse[nextNode].X, nodesToTraverse[nextNode].Y);
        }

        /// <summary>
        /// Checks if it's possible to travel straight from one note to the other
        /// </summary>
        /// <param name="currentPosition">The start-position on the map to check</param>
        /// <param name="newPosition">The end-position on the map to check</param>
        /// <returns></returns>
        private static bool PossibleToMoveBetween(Coordinates currentPosition, Node newPosition, Node[][] map)
        {
            List<Coordinates> passingCoordinates = BresenhamLine(currentPosition.X, currentPosition.Y, newPosition.X, newPosition.Y);
            foreach (Coordinates coordinate in passingCoordinates)
            {
                if (map[coordinate.X][coordinate.Y].Obstacle)
                {
                    // There is a wall in the colliding line
                    return false;
                }
            }
            // No wall is found between these coordinates
            return true;
        }

        /// <summary>
        /// Returns a List of Coordinates which collide with the line between a startpoint and an endpoint
        /// </summary>
        /// <param name="startPointX">The x coordinate of the startpoint</param>
        /// <param name="startPointY">The y coordinate of the startpoint</param>
        /// <param name="endPointX">The x coordinate of the endpoint</param>
        /// <param name="endPointY">The y coordinate of the endpoint</param>
        /// <returns>List of intersecting Coordinates</returns>
        private static List<Coordinates> BresenhamLine(int startPointX, int startPointY, int endPointX, int endPointY)
        {
            List<Coordinates> result = new List<Coordinates>();

            // If the difference in height of the line is higher than the difference of width, it will be a steep line.
            bool steep = Math.Abs(endPointY - startPointY) > Math.Abs(endPointX - startPointX);
            if (steep)
            {
                Swap(ref startPointX, ref startPointY);
                Swap(ref endPointX, ref endPointY);
            }
            if (startPointX > endPointX)
            {
                Swap(ref startPointX, ref endPointX);
                Swap(ref startPointY, ref endPointY);
            }

            int deltax = endPointX - startPointX;
            int deltay = Math.Abs(endPointY - startPointY);
            int error = 0;
            int ystep;
            int y = startPointY;
            if (startPointY < endPointY)
            {
                ystep = 1;
            }
            else
            {
                ystep = -1;
            }

            for (int x = startPointX; x <= endPointX; x++)
            {
                if (steep)
                {
                    result.Add(new Coordinates(y, x));
                }
                else
                {
                    result.Add(new Coordinates(x, y));
                }
                error += deltay;
                if (2 * error >= deltax)
                {
                    if (steep)
                    {
                        result.Add(new Coordinates(y, x+1));
                    }
                    else
                    {
                        result.Add(new Coordinates(x+1, y));
                    }

                    y += ystep;
                    error -= deltax;
                }
            }
            return result;
        }

        /// <summary>
        ///Swap the values of A and B
        /// </summary>
        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}
