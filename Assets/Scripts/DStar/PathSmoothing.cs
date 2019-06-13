﻿/**
 * Authors: Benji, Chiel
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public static class PathSmoothing
{
    /// <summary>
    /// The tile-offset used for the map.
    /// </summary>
    public const float TILE_OFFSET = 0.5f;

    /// <summary>
    /// Returns the farthest coordinate the entity can reach from its current position.
    /// </summary>
    /// <param name="currentPosition">The current position of the entity.</param>
    /// <param name="coordinatesToTraverse">A list containing all the coordinates the entity should traverse to reach its goal.</param>
    /// <param name="map">The map the entity knows so far.</param>
    /// <param name="entityWidth">The with of the entity.</param>
    public static Vector2Int FarthestCoordinateToReach(Vector2 currentPosition, List<Vector2Int> coordinatesToTraverse, NavigationGraph map, float entityWidth)
    {
        if (coordinatesToTraverse.Count == 0)
        {
            return new Vector2Int();
        }

        int nextNode = 0;
        int previousNextNode = nextNode;
        int goalNode = coordinatesToTraverse.Count - 1;
        bool possibleToMoveHere = true;

        // When it is possible to move from current position and the new position AND the new position is not the final destination
        while (possibleToMoveHere && nextNode != goalNode)
        {
            // Save current new point for if the next one fails.
            previousNextNode = nextNode;
            // Get new point that is closer to the destination.
            nextNode++;
            // For if the path does not lead to the goalNode (Goalnode is unreachable)
            // Take the last node in the list of CoordinatesToTraverse
            if (coordinatesToTraverse.Count == nextNode)
            {
                continue;
            }
            // Skip this node if it's on the same row or column as the current position.
            // In this case we won't have to recalculate if it's possible to move somewhere since the straight paths are already calculated by the D* algorithm.
            if (coordinatesToTraverse[nextNode].x == currentPosition.x || coordinatesToTraverse[nextNode].y == currentPosition.y)
            {
                continue;
            }
            // Check if it is possible to move to the next position.
            possibleToMoveHere = PossibleToMoveBetween(currentPosition, coordinatesToTraverse[nextNode], map, entityWidth, entityWidth);
        }

        // If it is not possible to move to the next position.
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
    private static bool PossibleToMoveBetween(Vector2 currentPosition, Vector2 newPosition, NavigationGraph map, float xOffset, float yOffset)
    {
        float leftSideFromEntity = -(xOffset / 2);
        float rightSideFromEntity = +(xOffset / 2);
        float upperSideFromEntity = +(yOffset / 2);
        float lowerSideFromEntity = -(yOffset / 2);

        // If there is an obstacle on this line, return false since it's not possible to move here.
        if (CheckIfNodeContainsObstacle(Traverse(currentPosition.x, currentPosition.y + upperSideFromEntity, newPosition.x, newPosition.y + upperSideFromEntity), map) // Top of entity
            || CheckIfNodeContainsObstacle(Traverse(currentPosition.x + leftSideFromEntity, currentPosition.y, newPosition.x + leftSideFromEntity, newPosition.y), map) // Left of entity
            || CheckIfNodeContainsObstacle(Traverse(currentPosition.x + rightSideFromEntity, currentPosition.y, newPosition.x + rightSideFromEntity, newPosition.y), map) // Right of entity
            || CheckIfNodeContainsObstacle(Traverse(currentPosition.x, currentPosition.y + lowerSideFromEntity, newPosition.x, newPosition.y + lowerSideFromEntity), map)) // Bottom of entity
        {
            return false;
        }

        // No obstacles have been seen.
        return true;
    }

    /// <summary>
    /// Checks if one of the coordinates contains an obstacle
    /// </summary>
    private static bool CheckIfNodeContainsObstacle(List<Vector2Int> passingCoordinates, NavigationGraph map)
    {
        foreach (Vector2Int coordinate in passingCoordinates)
        {
            if (map.GetNode(coordinate).IsObstacle())
            {
                // There is a wall in the colliding line
                return true;
            }
        }
        // No wall is found between these coordinates
        return false;
    }

    private static List<Vector2Int> Traverse(float startPointX, float startPointY, float endPointX, float endPointY)
    {
        List<Vector2Int> passingCoordinates = new List<Vector2Int>();
        // Change the positions to fit our grid
        startPointX += TILE_OFFSET;
        startPointY += TILE_OFFSET;
        endPointX += TILE_OFFSET;
        endPointY += TILE_OFFSET;

        // The difference between the points
        float deltaX = Mathf.Abs(endPointX - startPointX);
        float deltaY = Mathf.Abs(endPointY - startPointY);

        // Initialize the starting points (floor to get to the right tile position)
        float tileX = (float)Math.Floor(startPointX);
        float tileY = (float)Math.Floor(startPointY);

        // Counter
        float counter = 0;

        // Whether the x should be incremented or decremented (left is decremented, right is incremented)
        // Whether the y should be incremented or decremented (down is decremented, up is incremented)
        int stepX, stepY;

        // The difference between t_next_horizontal and t_next_vertical;
        // if it’s positive, the next vertical tile is closer; if it’s negative,
        // the next horizontal tile is closer.
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
        else if (endPointX > startPointX) // If the entity has to move right
        {
            stepX = 1;
            counter += (float)(Math.Floor(endPointX)) - tileX;
            error = (Math.Floor(startPointX) + 1 - startPointX) * deltaY;
        }
        else // If the entity has to move left
        {
            stepX = -1;
            counter += tileX - (float)(Math.Floor(endPointX));
            error = (startPointX - Math.Floor(startPointX)) * deltaY;
        }
        #endregion

        // Calculate stepY
        #region Calculate stepY
        if (deltaY == 0) // only moves horizontally
        {
            stepY = 0;
            error = double.PositiveInfinity;
        }
        else if (endPointY > startPointY) // If the entity has to move up
        {
            stepY = 1;
            counter += (float)(Math.Floor(endPointY)) - tileY;
            error -= (Math.Floor(startPointY) + 1 - startPointY) * deltaX;
        }
        else // If the entity has to move down
        {
            stepY = -1;
            counter += tileY - (float)(Math.Floor(endPointY));
            error -= (startPointY - Math.Floor(startPointY)) * deltaX;
        }
        #endregion

        passingCoordinates.Add(new Vector2Int((int)(tileX), (int)(tileY)));

        // While the end of the line hasn't been reach
        while (counter > 0)
        {
            // If the error is bigger than 0
            if (error > 0)
            {
                // Go to the next vertical tile from this point
                tileY += stepY;
                error -= deltaX;
            }
            else if (error == 0) // If the line exactly crosses two tiles at once
            {
                tileX += stepX;
                tileY += stepY;
                error -= deltaX;
                error += deltaY;
                counter--;
            }
            else
            {
                // Go to the next horizontal tile from this point 
                tileX += stepX;
                error += deltaY;
            }

            passingCoordinates.Add(new Vector2Int((int)(tileX), (int)(tileY)));
            // Decrease the counter
            counter--;
        }
        return passingCoordinates;
    }

#if (UNITY_EDITOR)
    private static void DebugRectangle(float tileX, float tileY)
    {
        RectangleF rect = new RectangleF(tileX - TILE_OFFSET, tileY - TILE_OFFSET, 1, 1);
        Debug.DrawLine(new Vector3(rect.Left, 0, rect.Bottom), new Vector3(rect.Right, 0, rect.Bottom), UnityEngine.Color.white, 0.01f);
        Debug.DrawLine(new Vector3(rect.Left, 0, rect.Top), new Vector3(rect.Right, 0, rect.Top), UnityEngine.Color.white, 0.01f);
        Debug.DrawLine(new Vector3(rect.Left, 0, rect.Bottom), new Vector3(rect.Left, 0, rect.Top), UnityEngine.Color.white, 0.01f);
        Debug.DrawLine(new Vector3(rect.Right, 0, rect.Bottom), new Vector3(rect.Right, 0, rect.Top), UnityEngine.Color.white, 0.01f);
    }
#endif
}
