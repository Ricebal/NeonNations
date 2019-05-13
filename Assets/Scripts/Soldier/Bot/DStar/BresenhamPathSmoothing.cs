using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Soldier.Bot.DStar
{
    public static class BresenhamPathSmoothing
    {
        public static Coordinates FarthestNodeToReach(PointF currentPosition, List<Node> nodesToTraverse, Node[][] map, float xOffset, float yOffset)
        {
            int nextNode = 0;
            int previousNextNode = nextNode;
            int goalNode = nodesToTraverse.Count - 1;
            bool possibleToMoveHere = true;

            // When it's possible to move from current position and the new position AND the new position is not the final destination
            while (possibleToMoveHere && nextNode != goalNode)
            {
                // Save current new point for if the next one fails.
                previousNextNode = nextNode;
                // Get new point that is closer to the destination.
                nextNode++;
                // Check if it's possible to move to the next position
                possibleToMoveHere = PossibleToMoveBetween(currentPosition, new PointF(nodesToTraverse[nextNode].X, nodesToTraverse[nextNode].Y), map, xOffset, yOffset);
            }

            // If it's not possible to move to the next position
            if (!possibleToMoveHere)
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
        /// <param name="map">The map on which to check the positions</param>
        /// <returns></returns>
        private static bool PossibleToMoveBetween(PointF currentPosition, PointF newPosition, Node[][] map, float xOffset, float yOffset)
        {
            float tileOffset = 0;
            float leftSideFromBot = -(xOffset / 2);
            float rightSideFromBot = +(xOffset / 2);
            float upperSideFromBot = +(yOffset / 2);
            float lowerSideFromBot = -(yOffset / 2);

            // Top of bot
            // draw a 5-unit red line from the origin for .01 seconds
            Debug.DrawLine(new Vector3(currentPosition.X , 0, currentPosition.Y + upperSideFromBot), new Vector3(newPosition.X + tileOffset, 0, newPosition.Y + tileOffset + upperSideFromBot), UnityEngine.Color.red, 0.01f);
            List<Coordinates> passingCoordinates = TilesIntersectedByLine(currentPosition.X, currentPosition.Y + upperSideFromBot, newPosition.X + tileOffset, newPosition.Y + tileOffset + upperSideFromBot);
            // Left of bot
            // draw a 5-unit red line from the origin for .01 seconds
            Debug.DrawLine(new Vector3(currentPosition.X + leftSideFromBot, 0, currentPosition.Y  ), new Vector3(newPosition.X + tileOffset + leftSideFromBot, 0, newPosition.Y + tileOffset), UnityEngine.Color.yellow, 0.01f);
            passingCoordinates.AddRange(TilesIntersectedByLine(currentPosition.X + leftSideFromBot, currentPosition.Y  , newPosition.X + tileOffset, newPosition.Y + tileOffset));
            // Right of bot
            // draw a 5-unit red line from the origin for .01 seconds
            Debug.DrawLine(new Vector3(currentPosition.X + rightSideFromBot, 0, currentPosition.Y  ), new Vector3(newPosition.X + tileOffset + rightSideFromBot, 0, newPosition.Y + tileOffset  ), UnityEngine.Color.green, 0.01f);
            passingCoordinates.AddRange(TilesIntersectedByLine(currentPosition.X + rightSideFromBot, currentPosition.Y  , newPosition.X + tileOffset + rightSideFromBot, newPosition.Y + tileOffset  ));
            // Bottom of bot
            // draw a 5-unit red line from the origin for .01 seconds
            Debug.DrawLine(new Vector3(currentPosition.X , 0, currentPosition.Y + lowerSideFromBot), new Vector3(newPosition.X + tileOffset, 0, newPosition.Y + tileOffset + lowerSideFromBot), UnityEngine.Color.blue, 0.01f);
            passingCoordinates.AddRange(TilesIntersectedByLine(currentPosition.X , currentPosition.Y + lowerSideFromBot, newPosition.X + tileOffset , newPosition.Y + tileOffset + lowerSideFromBot));
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

        public static bool LineIntersectsRect(PointF currentPosition, PointF nextPosition, RectangleF tile)
        {
            bool lineCrossesTile = LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X, tile.Y), new PointF(tile.X + tile.Width, tile.Y)) ||
                   LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X + tile.Width, tile.Y), new PointF(tile.X + tile.Width, tile.Y + tile.Height)) ||
                   LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X + tile.Width, tile.Y + tile.Height), new PointF(tile.X, tile.Y + tile.Height)) ||
                   LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X, tile.Y + tile.Height), new PointF(tile.X, tile.Y)) ||
                   (tile.Contains(currentPosition) && tile.Contains(nextPosition));

            if (lineCrossesTile)
            {
                // Draw debug rectangle
                // Top
                Debug.DrawLine(new Vector3(tile.Left, 0.1f, tile.Top), new Vector3(tile.Right, 0.1f, tile.Top), UnityEngine.Color.white, 0.01f);
                // Left
                Debug.DrawLine(new Vector3(tile.Left, 0.1f, tile.Bottom), new Vector3(tile.Left, 0.1f, tile.Top), UnityEngine.Color.white, 0.01f);
                // Right
                Debug.DrawLine(new Vector3(tile.Right, 0.1f, tile.Bottom), new Vector3(tile.Right, 0.1f, tile.Top), UnityEngine.Color.white, 0.01f);
                // Bottom
                Debug.DrawLine(new Vector3(tile.Left, 0.1f, tile.Bottom), new Vector3(tile.Right, 0.1f, tile.Bottom), UnityEngine.Color.white, 0.01f);
            }
            return lineCrossesTile;
        }

        private static bool LineIntersectsLine(PointF currentPosition, PointF nextPosition, PointF rectanglePoint1, PointF rectanglePoint2)
        {
            float q = (currentPosition.Y - rectanglePoint1.Y) * (rectanglePoint2.X - rectanglePoint1.X) - (currentPosition.X - rectanglePoint1.X) * (rectanglePoint2.Y - rectanglePoint1.Y);
            float d = (nextPosition.X - currentPosition.X) * (rectanglePoint2.Y - rectanglePoint1.Y) - (nextPosition.Y - currentPosition.Y) * (rectanglePoint2.X - rectanglePoint1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (currentPosition.Y - rectanglePoint1.Y) * (nextPosition.X - currentPosition.X) - (currentPosition.X - rectanglePoint1.X) * (nextPosition.Y - currentPosition.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns a List of Coordinates that will be intersected by a line
        /// </summary>
        private static List<Coordinates> TilesIntersectedByLine(float startPointX, float startPointY, float endPointX, float endPointY)
        {
            List<Coordinates> results = new List<Coordinates>();
            float leftX = startPointX;
            float rightX = endPointX;
            float downY = startPointY;
            float upY = endPointY;

            // Set's the most left position to leftX
            if (startPointX > endPointX)
            {
                leftX = endPointX;
                rightX = startPointX;
            }
            // Set's the lowest position to downY
            if (startPointY > endPointY)
            {
                downY = endPointY;
                upY = startPointY;
            }
            // Round numbers to prevent weird detections
            downY = Mathf.Floor(downY);
            leftX = Mathf.Floor(leftX);
            upY = Mathf.Ceil(upY);
            rightX = Mathf.Ceil(rightX);

            float tileOffset = 0.5f; // The value of the map-offset

            // For each Tile in the Rectangle
            for (float i = leftX; i <= rightX; i++)
            {
                for(float j = downY; j <= upY; j++)
                {
                    
                    RectangleF tile = new RectangleF(i-tileOffset,j-tileOffset,1,1);
                    // Draw debug rectangle
                    // Top
                    Debug.DrawLine(new Vector3(tile.Left, 0, tile.Top), new Vector3(tile.Right, 0, tile.Top), UnityEngine.Color.gray, 0.01f, true);
                    // Left
                    Debug.DrawLine(new Vector3(tile.Left, 0, tile.Bottom), new Vector3(tile.Left, 0, tile.Top), UnityEngine.Color.gray, 0.01f, true);
                    // Right
                    Debug.DrawLine(new Vector3(tile.Right, 0, tile.Bottom), new Vector3(tile.Right, 0, tile.Top), UnityEngine.Color.gray, 0.01f, true);
                    // Bottom
                    Debug.DrawLine(new Vector3(tile.Left, 0, tile.Bottom), new Vector3(tile.Right, 0, tile.Bottom), UnityEngine.Color.gray, 0.01f, true);

                    // Check if it is intersected by the line
                    if (LineIntersectsRect(new PointF(startPointX, startPointY), new PointF(endPointX, endPointY), tile))
                    {
                        results.Add(new Coordinates((int)i, (int)j));
                    }
                }
            }

            return results;
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
