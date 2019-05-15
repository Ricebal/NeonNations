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
        public static Vector2Int FarthestCoordinateToReach(PointF currentPosition, List<Vector2Int> coordinatesToTraverse, NavigationGraph map, float entityWidth)
        {
            int nextNode = 0;
            int previousNextNode = nextNode;
            int goalNode = coordinatesToTraverse.Count - 1;
            bool possibleToMoveHere = true;

            // When it's possible to move from current position and the new position AND the new position is not the final destination
            while (possibleToMoveHere && nextNode != goalNode)
            {
                // Save current new point for if the next one fails.
                previousNextNode = nextNode;
                // Get new point that is closer to the destination.
                nextNode++;
                // Check if it's possible to move to the next position
                possibleToMoveHere = PossibleToMoveBetween(currentPosition, new PointF(coordinatesToTraverse[nextNode].x, coordinatesToTraverse[nextNode].y), map, entityWidth, entityWidth);
            }

            // If it's not possible to move to the next position
            if (!possibleToMoveHere)
            {
                // Get back to previousPoint, since that's the last point that is reachable.
                nextNode = previousNextNode;
            }
            return new Vector2Int(coordinatesToTraverse[nextNode].x, coordinatesToTraverse[nextNode].y);
        }

        /// <summary>
        /// Checks if it's possible to travel straight from one note to the other
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
            // Top of entity
            TilesIntersectedByLine(currentPosition.X, currentPosition.Y + upperSideFromEntity, newPosition.X, newPosition.Y + upperSideFromEntity, ref passingCoordinates);
            // Left of entity
            TilesIntersectedByLine(currentPosition.X + leftSideFromEntity, currentPosition.Y, newPosition.X + leftSideFromEntity, newPosition.Y, ref passingCoordinates);
            // Right of entity
            TilesIntersectedByLine(currentPosition.X + rightSideFromEntity, currentPosition.Y, newPosition.X + rightSideFromEntity, newPosition.Y, ref passingCoordinates);
            // Bottom of entity
            TilesIntersectedByLine(currentPosition.X, currentPosition.Y + lowerSideFromEntity, newPosition.X, newPosition.Y + lowerSideFromEntity, ref passingCoordinates);
            foreach (Vector2Int coordinate in passingCoordinates)
            {
                if (map.GetNode(coordinate.x, coordinate.y).Obstacle)
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
            // 
            float q = (currentPosition.Y - rectanglePoint1.Y) * (rectanglePoint2.X - rectanglePoint1.X) - (currentPosition.X - rectanglePoint1.X) * (rectanglePoint2.Y - rectanglePoint1.Y);
            // 
            float delta = (nextPosition.X - currentPosition.X) * (rectanglePoint2.Y - rectanglePoint1.Y) - (nextPosition.Y - currentPosition.Y) * (rectanglePoint2.X - rectanglePoint1.X);

            if (delta == 0)
            {
                return false;
            }

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
        /// Returns a List of Coordinates that will be intersected by a line
        /// </summary>
        private static void TilesIntersectedByLine(float startPointX, float startPointY, float endPointX, float endPointY, ref List<Vector2Int> passingCoordinates)
        {
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
