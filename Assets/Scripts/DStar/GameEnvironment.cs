/**
 * Authors: Chiel, Benji
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameEnvironment : ScriptableObject
{
    private Map m_map;
    private List<Tile> m_listOfObstacles = new List<Tile>();
    private const int BOT_RANGE = 2;
    private const int SONAR_RANGE = 7;
    private const int BULLET_RANGE = 1;
    private const int IMPACT_RANGE = 2;
    private const int DEFAULT_RANGE = 5;

    /// <summary>
    /// Acts as a constructor for GameEnvironment, since it's real constructor can't be used because it's a ScriptableObject
    /// It uses Init to set the values
    /// </summary>
    /// <param name="map">Map to set</param>
    /// <param name="list">List to set</param>
    /// <returns>A new GameEnvironment with the set values</returns>
    public static GameEnvironment CreateInstance(Map map, List<Tile> list)
    {
        var data = CreateInstance<GameEnvironment>();
        data.Init(map, list);
        return data;
    }

    /// <summary>
    /// Initializes this GameEnvironment
    /// </summary>
    /// <param name="map">Map to set</param>
    /// <param name="list">List to set</param>
    private void Init(Map map, List<Tile> list)
    {
        m_map = map;

        if (BoardManager.Singleton != null && BoardManager.Singleton.BreakableWalls != null)
        {
            GameObject wallParent = BoardManager.Singleton.BreakableWalls; // Get all breakable walls.
            for (int i = 0; i < wallParent.transform.childCount; i++)
            {
                GameObject wall = wallParent.transform.GetChild(i).gameObject;
                wall.GetComponent<BreakableWall>().WallDestroyedHandler += UpdateMap;
            }
        }

        m_listOfObstacles = list;
    }

    public void UpdateMap(Vector2Int coordinates)
    {
        m_map.UpdateMap(coordinates);
    }

    /// <summary>
    /// Gives the current map
    /// </summary>
    /// <returns>Map Map</returns>
    public Map GetMap()
    {
        return m_map;
    }

    /// <summary>
    /// Gets the list of obstacles the pathfinding algorithm should worry about
    /// </summary>
    /// <returns>List of Tile types containing the obstacles</returns>
    public List<Tile> GetList()
    {
        return m_listOfObstacles;
    }

    /// <summary>
    /// Gets the content at a certain position
    /// </summary>
    /// <param name="pos">Position of node</param>
    /// <returns>Tile enum containing the type of tile</returns>
    public Tile GetNode(Vector2Int pos)
    {
        return m_map.TileMap[pos.x][pos.y];
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
            if (coordinate.x < 0 || coordinate.y < 0 || coordinate.x >= m_map.TileMap.Length || coordinate.y >= m_map.TileMap[0].Length)
            {
                coordinatesToDelete.AddLast(coordinate);
                continue;
            }
            // If the coordinates are inside the map, check if the coordinate contains an obstacle
            if (!m_listOfObstacles.Contains(m_map.TileMap[coordinate.x][coordinate.y]))
            {
                coordinatesToDelete.AddLast(coordinate);
            }
        }
        // Delete the coordinates that contain no obstacles
        foreach (Vector2Int coordinate in coordinatesToDelete)
        {
            coordinates.Remove(coordinate);
        }
    }

    /// <summary>
    /// Get all Coordinates that are illuminated by light on the bot's screen
    /// </summary>
    public HashSet<Vector2Int> GetIlluminatedCoordinates(Vector2Int botCoordinates)
    {
        HashSet<Vector2Int> illuminatedCoordinates = GetCoordinatesAroundBot(botCoordinates); // Because of the spotlight around the bot
        GetCoordinatesAroundLights(ref illuminatedCoordinates, botCoordinates); // All other lights visible on screen
        return illuminatedCoordinates;
    }

    public Soldier GetClosestIlluminatedEnemy(Soldier soldier, List<Soldier> enemies)
    {
        HashSet<Vector2Int> list = GetIlluminatedCoordinates(ConvertGameObjectToCoordinates(soldier.transform));

        float minDist = Mathf.Infinity;
        Soldier closestEnemy = null;
        enemies.ForEach(enemy =>
        {
            float dist = Vector3.Distance(soldier.transform.position, enemy.transform.position);
            if (list.Contains(ConvertGameObjectToCoordinates(enemy.transform)) && dist < minDist)
            {
                closestEnemy = enemy;
                minDist = dist;
            }
        });

        return closestEnemy;
    }

    public List<Soldier> GetIlluminatedEnemies(Soldier soldier, List<Soldier> enemies)
    {
        HashSet<Vector2Int> list = GetIlluminatedCoordinates(ConvertGameObjectToCoordinates(soldier.transform));
        List<Soldier> result = new List<Soldier>();
        enemies.ForEach(enemy =>
        {
            if (list.Contains(ConvertGameObjectToCoordinates(enemy.transform)))
            {
                result.Add(enemy);
            }
        });

        return result;
    }

    /// <summary>
    /// Get the Coordinates that are illuminated by the spotlight on the bot
    /// </summary>
    private HashSet<Vector2Int> GetCoordinatesAroundBot(Vector2Int botCoordinates)
    {
        HashSet<Vector2Int> currentCoordinatesInSight = new HashSet<Vector2Int>();
        return GetCoordinatesInRange(botCoordinates, BOT_RANGE, currentCoordinatesInSight);
    }

    /// <summary>
    /// Returns a linked list containing all map-coordinates that should be illuminated by the object
    /// </summary>
    /// <param name="objectCoordinates">The Coordinates of the object you want to check the light from</param>
    /// <param name="range">The range of the light that's emitted by the object</param>
    /// <param name="coordinatesInRange">The Coordinates that have to be checked</param>
    private HashSet<Vector2Int> GetCoordinatesInRange(Vector2Int objectCoordinates, int range, HashSet<Vector2Int> coordinatesInRange)
    {
        for (int i = -range; i <= range; i++)
        {
            for (int j = -range; j <= range; j++)
            {
                int x = objectCoordinates.x + i;
                int y = objectCoordinates.y + j;

                // Check if coordinate is inside the map
                if (x < 0 || x >= m_map.TileMap.Length || y < 0 || y >= m_map.TileMap[0].Length)
                {
                    continue;
                }
                coordinatesInRange.Add(new Vector2Int(x, y));
            }
        }
        return coordinatesInRange;
    }

    /// <summary>
    /// Get coordinates that are illuminated by lights
    /// </summary>
    /// <param name="illuminatedCoordinates">LinkedListist to which the illuminated Coordinates should be added</param>
    private void GetCoordinatesAroundLights(ref HashSet<Vector2Int> illuminatedCoordinates, Vector2Int botCoordinates)
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

                    range = Mathf.Clamp((int)Math.Floor(light.range), 0, DEFAULT_RANGE);
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
        int screenWidth = 12;
        int screenHeight = 7;
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
