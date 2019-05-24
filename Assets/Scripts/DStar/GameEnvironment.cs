using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEnvironment : ScriptableObject
{
    private Tile[][] m_map;
    private const int SONAR_RANGE = 5;
    private const int BULLET_RANGE = 1;
    private const int IMPACT_RANGE = 2;

    private void OnEnable()
    {
    }

    /// <summary>
    /// Gives the current map
    /// </summary>
    /// <returns>int[][] Map</returns>
    public Tile[][] GetMap()
    {
        return m_map;
    }

    public void SetMap(Tile[][] map)
    {
        m_map = map;
    }

    public Tile GetNode(int x, int y)
    {
        return m_map[x][y];
    }

    /// <summary>
    /// Checks if the Coordinates in sight are walls or empty spaces.
    /// </summary>
    /// <param name="coordinates">The Coordinates that should be checked</param>
    private void FilterCoordinatesWithObstacle(ref LinkedList<Vector2Int> coordinates)
    {
        LinkedList<Vector2Int> coordinatesToDelete = new LinkedList<Vector2Int>();
        foreach (Vector2Int coordinate in coordinates)
        {
            // If the coordinates are outside the map they shouldn't be checked (to prevent out of index exception)
            if (coordinate.x < 0 || coordinate.y < 0 || coordinate.x >= m_map.Length || coordinate.y >= m_map[0].Length)
            {
                coordinatesToDelete.AddLast(coordinate);
                continue;
            }
            // If the coordinates are inside the map, check if the coordinate contains an obstacle
            if (m_map[coordinate.x][coordinate.y] == Tile.Floor)
            {
                coordinatesToDelete.AddLast(coordinate);
            }
        }
        // Delete the coordintes that contain no obstacles
        foreach (Vector2Int coordinate in coordinatesToDelete)
        {
            coordinates.Remove(coordinate);
        }
    }

    /// <summary>
    /// Get all Coordinates that are illuminated by light on the bot's screen
    /// </summary>
    public LinkedList<Vector2Int> GetIlluminatedCoordinates(Vector2Int botCoordinates)
    {
        LinkedList<Vector2Int> illuminatedCoordinates = GetCoordinatesAroundBot(botCoordinates); // Because of the spotlight around the bot
        GetCoordinatesAroundLights(ref illuminatedCoordinates, botCoordinates); // All other lights visible on screen
        return illuminatedCoordinates;
    }

    /// <summary>
    /// Get the Coordinates that are illuminated by the spotlight on the bot
    /// </summary>
    private LinkedList<Vector2Int> GetCoordinatesAroundBot(Vector2Int botCoordinates)
    {
        LinkedList<Vector2Int> currentCoordinatesInSight = new LinkedList<Vector2Int>();
        return GetCoordinatesInRange(botCoordinates, 2, currentCoordinatesInSight);
    }

    /// <summary>
    /// Returns a linked list containing all map-coordinates that should be illuminated by the object
    /// </summary>
    /// <param name="objectCoordinates">The Coordinates of the object you want to check the light from</param>
    /// <param name="range">The range of the light that's emitted by the object</param>
    /// <param name="coordinatesInRange">The Coordinates that have to be checked</param>
    private LinkedList<Vector2Int> GetCoordinatesInRange(Vector2Int objectCoordinates, int range, LinkedList<Vector2Int> coordinatesInRange)
    {
        range = Mathf.Clamp(range, 0, 5);
        for (int i = -range; i < range; i++)
        {
            for (int j = -range; j < range; j++)
            {
                int x = objectCoordinates.x + i;
                int y = objectCoordinates.y + j;

                // Check if coordinate is inside the map
                if (x < 0 || x >= m_map.Length || y < 0 || y >= m_map[0].Length)
                {
                    continue;
                }
                // Check if coordinate is already added to the list
                if (!coordinatesInRange.Any(coordinate => coordinate.x == x && coordinate.y == y))
                {
                    coordinatesInRange.AddLast(new Vector2Int(x, y));
                }
            }
        }
        return coordinatesInRange;
    }

    /// <summary>
    /// Get coordinates that are illuminated by lights
    /// </summary>
    /// <param name="illuminatedCoordinates">LinkedListist to which the illuminated Coordinates should be added</param>
    private void GetCoordinatesAroundLights(ref LinkedList<Vector2Int> illuminatedCoordinates, Vector2Int botCoordinates)
    {
        // Get all lights in sight of the bot
        List<Light> lightsInSight = GetLights(botCoordinates);

        // Get all illuminated coordinates
        foreach (Light light in lightsInSight)
        {
            int range = 0;
            // The specific range of the lights.
            // Because the sonar and the impact-effect use such a big range,
            // the bot would see way to much with those values.
            // This is why those values are fixed.
            // Also, this way the prefabs could be changed without having any consequences for the bots.
            switch (light.transform.parent.tag)
            {
                case "Sonar":
                    range = SONAR_RANGE;
                    break;
                case "Bullet":
                    range = BULLET_RANGE;
                    break;
                case "Impact":
                    range = IMPACT_RANGE;
                    break;
                default:
                    range = (int)Math.Floor(light.range);
                    break;
            }
            GetCoordinatesInRange(ConvertGameObjectToCoordinates(light.transform), range, illuminatedCoordinates);
        }
    }

    /// <summary>
    /// Get all lights in view of the bot
    /// </summary>
    private List<Light> GetLights(Vector2Int botCoordinates)
    {
        // Get's all lights in the scene 
        Light[] lights = FindObjectsOfType<Light>();
        // Filter lights by point light and check if the position is inside the view
        List<Light> pointLights = lights.Where(light => light.type == UnityEngine.LightType.Point && CheckIfPositionIsInsideView(light.transform.position, botCoordinates)).ToList();
        // Remove point lights of the players and bots
        return pointLights.Where(light => light.transform.parent.tag != "Player").ToList();
    }

    /// <summary>
    /// Checks if a position is inside the view of the bot
    /// </summary>
    /// <param name="position">The position you want to check</param>
    private bool CheckIfPositionIsInsideView(Vector3 position, Vector2Int botCoordinates)
    {
        int screenWidth = 11;
        int screenHeight = 5;
        float viewLeft = botCoordinates.x - screenWidth;
        float viewUp = botCoordinates.y + screenHeight;
        float viewRight = viewLeft + screenWidth * 2;
        float viewDown = viewUp - screenHeight * 2;
        float posX = position.x;
        float posZ = position.z;
        return (viewLeft <= posX && posX <= viewRight && viewDown <= posZ && posZ <= viewUp);
    }

    /// <summary>
    /// Converts the transform of a gameobject to coordinates on the map
    /// </summary>
    /// <param name="transform">The transform of the gameobject from which you want the coordinates</param>
    public Vector2Int ConvertGameObjectToCoordinates(Transform transform)
    {
        return new Vector2Int((int)Math.Round(transform.position.x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(transform.position.z, 0, MidpointRounding.AwayFromZero));
    }
}
