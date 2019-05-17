using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Soldier.Bot.DStar
{
    public static class PathSmoothing
    {
        /// <summary>
        /// Returns the farthest coordinate the entity can reach from its current position.
        /// </summary>
        /// <param name="currentPosition">The current position of the entity.</param>
        /// <param name="coordinatesToTraverse">A list containing all the coordinates the entity should traverse to reach its goal.</param>
        /// <param name="map">The map the entity knows so far.</param>
        /// <param name="entityWidth">The with of the entity.</param>
        public static Vector2Int FarthestCoordinateToReach(PointF currentPosition, List<Vector2Int> coordinatesToTraverse, NavigationGraph map, float entityWidth)
        {
            int nextNode = 0;
            int previousNextNode = nextNode;
            int goalNode = coordinatesToTraverse.Count - 1;
            bool possibleToMoveHere = true;

            // When it is possible to move from current position and the new position AND the new position is not the final destination
            //while (possibleToMoveHere && nextNode != goalNode)
            //{
            //    // Save current new point for if the next one fails.
            //    previousNextNode = nextNode;
            //    // Get new point that is closer to the destination.
            //    nextNode++;
            //    // Check if it is possible to move to the next position
            //    possibleToMoveHere = PossibleToMoveBetween(currentPosition, new PointF(coordinatesToTraverse[nextNode].x, coordinatesToTraverse[nextNode].y), map, entityWidth, entityWidth);
           // }

            possibleToMoveHere = PossibleToMoveBetween(currentPosition, new PointF(coordinatesToTraverse[goalNode].x, coordinatesToTraverse[goalNode].y), map, entityWidth, entityWidth);

            // If it is not possible to move to the next position
            if (!possibleToMoveHere)
            {
                // Get back to previousPoint, since that is the last point that is reachable.
                nextNode = previousNextNode;
            }

            return new Vector2Int(coordinatesToTraverse[nextNode].x, coordinatesToTraverse[nextNode].y);
        }

        /// <summary>
        /// Checks if it is possible to travel straight from one note to the other
        /// </summary>
        /// <param name="currentPosition">The start-position on the map to check</param>
        /// <param name="newPosition">The end-position on the map to check</param>
        /// <param name="map">The map on which to check the positions</param>
        /// <returns></returns>
        private static bool PossibleToMoveBetween(PointF currentPosition, PointF newPosition, NavigationGraph map, float xOffset, float yOffset)
        {
            float leftSideFromEntity = -(xOffset / 2);
            float rightSideFromEntity = +(xOffset / 2);
            float upperSideFromEntity = +(yOffset / 2);
            float lowerSideFromEntity = -(yOffset / 2);
            List<Vector2Int> passingCoordinates = new List<Vector2Int>();
            Traverse(currentPosition.X, currentPosition.Y, newPosition.X, newPosition.Y, ref passingCoordinates);

            Debug.DrawLine(new Vector3(currentPosition.X, 0, currentPosition.Y), new Vector3(newPosition.X, 0, newPosition.Y), UnityEngine.Color.red, 0.01f);
            //Debug.DrawLine(new Vector3(currentPosition.X + leftSideFromEntity, 0, currentPosition.Y), new Vector3(newPosition.X + leftSideFromEntity, 0, newPosition.Y), UnityEngine.Color.yellow, 0.01f);
            //Debug.DrawLine(new Vector3(currentPosition.X + rightSideFromEntity, 0, currentPosition.Y), new Vector3(newPosition.X + rightSideFromEntity, 0, newPosition.Y), UnityEngine.Color.green, 0.01f);
            //Debug.DrawLine(new Vector3(currentPosition.X, 0, currentPosition.Y + lowerSideFromEntity), new Vector3(newPosition.X, 0, newPosition.Y + lowerSideFromEntity), UnityEngine.Color.blue, 0.01f);

            //// Top of entity
            //Traverse(currentPosition.X, currentPosition.Y + upperSideFromEntity, newPosition.X, newPosition.Y + upperSideFromEntity, ref passingCoordinates);
            //// Left of entity
            //Traverse(currentPosition.X + leftSideFromEntity, currentPosition.Y, newPosition.X + leftSideFromEntity, newPosition.Y, ref passingCoordinates);
            //// Right of entity
            //Traverse(currentPosition.X + rightSideFromEntity, currentPosition.Y, newPosition.X + rightSideFromEntity, newPosition.Y, ref passingCoordinates);
            //// Bottom of entity
            //Traverse(currentPosition.X, currentPosition.Y + lowerSideFromEntity, newPosition.X, newPosition.Y + lowerSideFromEntity, ref passingCoordinates);
            foreach (Vector2Int coordinate in passingCoordinates)
            {
                if (map.GetNode(coordinate).IsObstacle())
                {
                    // There is a wall in the colliding line
                    return false;
                }
            }
            // No wall is found between these coordinates
            return true;
        }

        /// <summary>
        /// Check if line intersects a triangle
        /// </summary>
        /// <param name="currentPosition">The start-position of the line</param>
        /// <param name="nextPosition">The end-position of the line</param>
        /// <param name="tile">The rectangle you want to check for collision</param>
        public static bool LineIntersectsRect(PointF currentPosition, PointF nextPosition, RectangleF tile)
        {
            return LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X, tile.Y), new PointF(tile.X + tile.Width, tile.Y)) ||
                   LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X + tile.Width, tile.Y), new PointF(tile.X + tile.Width, tile.Y + tile.Height)) ||
                   LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X + tile.Width, tile.Y + tile.Height), new PointF(tile.X, tile.Y + tile.Height)) ||
                   LineIntersectsLine(currentPosition, nextPosition, new PointF(tile.X, tile.Y + tile.Height), new PointF(tile.X, tile.Y)) ||
                   (tile.Contains(currentPosition) && tile.Contains(nextPosition));
        }

        /// <summary>
        /// Checks if two lines intersect with each other
        /// </summary>
        private static bool LineIntersectsLine(PointF currentPosition, PointF nextPosition, PointF rectanglePoint1, PointF rectanglePoint2)
        {
            // The first point of line QR
            float q = (currentPosition.Y - rectanglePoint1.Y) * (rectanglePoint2.X - rectanglePoint1.X) - (currentPosition.X - rectanglePoint1.X) * (rectanglePoint2.Y - rectanglePoint1.Y);
            // Help-variable to calculate R
            float delta = (nextPosition.X - currentPosition.X) * (rectanglePoint2.Y - rectanglePoint1.Y) - (nextPosition.Y - currentPosition.Y) * (rectanglePoint2.X - rectanglePoint1.X);

            if (delta == 0)
            {
                return false;
            }
            
            // The second point of line QR
            float r = q / delta;

            q = (currentPosition.Y - rectanglePoint1.Y) * (nextPosition.X - currentPosition.X) - (currentPosition.X - rectanglePoint1.X) * (nextPosition.Y - currentPosition.Y);
            float s = q / delta;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Returns the fractional part of a float
        /// For example, Frac(1.3) = 0.3, Frac(-1.7)=0.3
        /// </summary>
        public static float Frac(float value)
        {
            return value - (float)Math.Truncate(value);
        }

        //private static float deltaHelper(float start, float end)
        //{
        //    int tile = (int)Math.Abs(start);
        //    int destinationTile;// = (int)Math.Abs(end);
        //    float deltaT;
        //    if(end > 0)
        //}
        //private static void RayCast(float startPointX, float startPointY, float endPointX, float endPointY, ref List<Vector2Int> passingCoordinates)
        //{
        //    // The difference between the points
        //    float deltaX = Mathf.Abs(endPointX - startPointX);
        //    float deltaY = Mathf.Abs(endPointY - startPointY);

        //    // The directions
        //    int dirSignX = Math.Sign(deltaX);
        //    int dirSignY = Math.Sign(deltaY);

        //    float tileOffsetX = 
        //}

        private static void Traverse(float startPointX, float startPointY, float endPointX, float endPointY, ref List<Vector2Int> passingCoordinates)
        {
            float tileOffset = 0.5f;

            //startPointX += tileOffset;
            //startPointY += tileOffset;
            //endPointX += tileOffset;
            //endPointY += tileOffset;

            // The difference between the points
            float deltaX = Mathf.Abs(endPointX - startPointX);
            float deltaY = Mathf.Abs(endPointY - startPointY);

            // Initialize the starting points (floor to get to the right tile position)
            float x = (float)Math.Floor(startPointX + tileOffset)- tileOffset;
            float y = (float)Math.Floor(startPointY + tileOffset)- tileOffset;

            // Counter
            float counter = 1;

            // Whether the x should be incremented or decremented (left is decremented, right is incremented)
            // Whether the y should be incremented or decremented (down is decremented, up is incremented)
            int stepX, stepY;

            // The difference between t_next_horizontal and t_next_vertical;
            // if it’s positive, we know one is closer; if it’s negative,
            // the other is closer. We add or subtract dt_dx and dt_dy as appropriate when we move.
            // If error > 0; Then move to next vertical tile;
            // If error < 0; Then move to next horizontal tile;
            double error;

            // Calculate stepX
            #region Calculate stepX
            if (deltaX == 0) // Only moves vertically
            {
                stepX = 0;
                error = double.PositiveInfinity;
            }
            else if (endPointX > startPointX) // Goes right
            {
                stepX = 1;
                counter += (float)(Math.Floor(endPointX)) - x;
                error = (Math.Floor(startPointX) + 1 - startPointX) * deltaY + tileOffset;
            }
            else // Goes left
            {
                stepX = -1;
                counter += x - (float)(Math.Floor(endPointX));
                error = (startPointX - Math.Floor(startPointX)) * deltaY - tileOffset;
            }
#endregion

            // Calculate stepY
            #region Calculate stepY
            if (deltaY == 0) // only moves horizontally
            {
                stepY = 0;
                error = double.PositiveInfinity;
            }
            else if (endPointY > startPointY) // Goes up
            {
                stepY = 1;
                counter += (float)(Math.Floor(endPointY)) - y;
                error -= (Math.Floor(startPointY) + 1 - startPointY) * deltaX - tileOffset;
            }
            else // Goes down
            {
                stepY = -1;
                counter += y - (float)(Math.Floor(endPointY));
                error -= (startPointY - Math.Floor(startPointY)) * deltaX + tileOffset;
            }
            #endregion

            RectangleF rect = new RectangleF(x, y, 1, 1);
            Debug.DrawLine(new Vector3(rect.Left, 0, rect.Bottom), new Vector3(rect.Right, 0, rect.Bottom), UnityEngine.Color.white, 0.01f);
            Debug.DrawLine(new Vector3(rect.Left, 0, rect.Top), new Vector3(rect.Right, 0, rect.Top), UnityEngine.Color.white, 0.01f);
            Debug.DrawLine(new Vector3(rect.Left, 0, rect.Bottom), new Vector3(rect.Left, 0, rect.Top), UnityEngine.Color.white, 0.01f);
            Debug.DrawLine(new Vector3(rect.Right, 0, rect.Bottom), new Vector3(rect.Right, 0, rect.Top), UnityEngine.Color.white, 0.01f);

            // If the coordinate isn't know already
            if (!passingCoordinates.Any(c => c.x == Math.Ceiling(x) && c.y == Math.Ceiling(y)))
            {
                passingCoordinates.Add(new Vector2Int((int)Math.Ceiling(x), (int)Math.Ceiling(y)));
            }

            // While the end of the line hasn't been reach
            while (counter>0)
            {
                // If the error is bigger than 0
                if (error > 0)
                {
                    // Go to the next vertical tile from this point
                    y += stepY;
                    error -= deltaX;
                }
                else if (error == 0) // If the line exactly crosses two tiles at once
                {
                    x += stepX;
                    y += stepY;
                    error -= deltaX;
                    error += deltaY;
                    counter--;
                }
                else{
                    // Go to the next horizontal tile from this point 
                    x += stepX;
                    error += deltaY;
                }
                rect = new RectangleF(x, y, 1, 1);
                Debug.DrawLine(new Vector3(rect.Left, 0, rect.Bottom), new Vector3(rect.Right, 0, rect.Bottom), UnityEngine.Color.white, 0.01f);
                Debug.DrawLine(new Vector3(rect.Left, 0, rect.Top), new Vector3(rect.Right, 0, rect.Top), UnityEngine.Color.white, 0.01f);
                Debug.DrawLine(new Vector3(rect.Left, 0, rect.Bottom), new Vector3(rect.Left, 0, rect.Top), UnityEngine.Color.white, 0.01f);
                Debug.DrawLine(new Vector3(rect.Right, 0, rect.Bottom), new Vector3(rect.Right, 0, rect.Top), UnityEngine.Color.white, 0.01f);

                // If the coordinate isn't know already
                if (!passingCoordinates.Any(c => c.x == Math.Ceiling(x) && c.y == Math.Ceiling(y)))
                {
                    passingCoordinates.Add(new Vector2Int((int)Math.Ceiling(x), (int)Math.Ceiling(y)));
                }
                // Decrease the counter
                counter--;
            }
        }

        /// <summary>
        /// Returns a List of Coordinates that will be intersected by a line
        /// </summary>
        private static void TilesIntersectedByLine2(float startPointX, float startPointY, float endPointX, float endPointY, ref List<Vector2Int> passingCoordinates)
        {
            float tileOffset = 0.5f; // The value of the map-offset
            // Set the x-coordinate of to the tile that contains startPointX
            float x = Mathf.Floor(startPointX) - tileOffset;
            // Set the y-coordinate of to the tile that contains startPointY
            float y = Mathf.Floor(startPointY) - tileOffset;
            // The final x-coordinate
            float finalCoordinateX = Mathf.Floor(endPointX) - tileOffset;
            // The final y-coordinate
            float finalCoordinateY = Mathf.Floor(endPointY) - tileOffset;
            // The distance between the startX and endX.
            float distanceX = endPointX - startPointX;
            // The distance between the startY and endY.
            float distanceY = endPointY - startPointY;
            // Whether the x should be incremented or decremented (left is decremented, right is incremented)
            int stepX = -1;
            if (distanceX >= 0)
            {
                stepX = 1;
            }
            // Whether the y should be incremented or decremented (down is decremented, up is incremented)
            int stepY = -1;
            if (distanceY >= 0)
            {
                stepY = 1;
            }

            // The range untill the ray will cross the first vertical boundary.
            float tDeltaX = 1 / distanceX;
            float tMaxX = tDeltaX * (1.0f - Frac(startPointX));

            // The range untill the ray will cross the first horizontal boundary.
            float tDeltaY = 1 / distanceY;
            float tMaxY = tDeltaY * (1.0f - Frac(startPointY));

            // While the endpoints hasn't been reached, keep in-/decreasing the x and y value.
            while (x != finalCoordinateX && y != finalCoordinateY)
            {
                // If the coordinate isn't know already
                if (!passingCoordinates.Any(c => c.x == x && c.y == y))
                {
                    passingCoordinates.Add(new Vector2Int((int)x, (int)y));
                }
                if (tMaxX < tMaxY)
                {
                    tMaxX = tMaxX + tDeltaX;
                    x = x + stepX;
                }
                else
                {
                    tMaxY = tMaxY + tDeltaY;
                    y = y + stepY;
                }
            }
        }

        /// <summary>
        /// Returns a List of Coordinates that will be intersected by a line
        /// </summary>
        private static void TilesIntersectedByLine(float startPointX, float startPointY, float endPointX, float endPointY, ref List<Vector2Int> passingCoordinates)
        {
            float leftX = startPointX;
            float rightX = endPointX;
            float downY = startPointY;
            float upY = endPointY;

            // Sets the most left position to leftX
            if (startPointX > endPointX)
            {
                leftX = endPointX;
                rightX = startPointX;
            }
            // Sets the lowest position to downY
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
                    // If the coordinate isn't know already
                    if(!passingCoordinates.Any(c => c.x == i && c.y == j))
                    {
                        RectangleF tile = new RectangleF(i-tileOffset,j-tileOffset,1,1);

                        // Check if it is intersected by the line
                        if (LineIntersectsRect(new PointF(startPointX, startPointY), new PointF(endPointX, endPointY), tile))
                        {
                                passingCoordinates.Add(new Vector2Int((int)i, (int)j));
                        }
                    }
                }
            }
        }
    }
}
