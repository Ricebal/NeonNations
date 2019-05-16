using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Soldier.Bot.DStar
{
    public class GameEnvironment : MonoBehaviour
    {
        private GameObject m_bot;
        private static int[][] m_map;

        public GameEnvironment(GameObject bot)
        {
            m_map = GameObject.Find("GameManager").GetComponent<BoardManager>().GetTileMap();
            m_bot = bot;

        }
        

        /// <summary>
        /// Gives the current map
        /// </summary>
        /// <returns>int[][] Map</returns>
        public int[][] GetMap()
        {
            return m_map;
        }

        public int GetNode(int x, int y)
        {
            return m_map[x][y];
        }

        public static void UpdateNode(int x, int y, int newNode)
        {
            m_map[x][y] = newNode;
        }

        // Return the obstacles the bot should be able to see
        public LinkedList<Vector2Int> GetPositionsInVision()
        {
            // First get all coordinates that are illuminated
            LinkedList<Vector2Int> currentCoordinatesInSight = GetIlluminatedCoordinates();
            // Than check the map-layout to see if the illuminated parts are walls or empty spaces
            //FilterCoordinatesWithObstacle(ref currentCoordinatesInSight);
            // Return the result
            return currentCoordinatesInSight;
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
                if (m_map[coordinate.x][coordinate.y] == 0)
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
        private LinkedList<Vector2Int> GetIlluminatedCoordinates()
        {
            LinkedList<Vector2Int> illuminatedCoordinates = GetCoordinatesAroundBot(); // Because of the spotlight around the bot
            GetCoordinatesAroundLights(ref illuminatedCoordinates); // All other lights visible on screen
            return illuminatedCoordinates;
        }

        /// <summary>
        /// Get the Coordinates that are illuminated by the spotlight on the bot
        /// </summary>
        private LinkedList<Vector2Int> GetCoordinatesAroundBot()
        {
            LinkedList<Vector2Int> currentCoordinatesInSight = new LinkedList<Vector2Int>();
            Vector2Int botCoordinate = ConvertGameObjectToCoordinates(m_bot.transform);
            return GetCoordinatesInRange(botCoordinate, 2, currentCoordinatesInSight);
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
        private void GetCoordinatesAroundLights(ref LinkedList<Vector2Int> illuminatedCoordinates)
        {
            // Get all lights in sight of the bot
            List<Light> lightsInSight = GetLights();

            // Get all illuminated coordinates
            foreach (Light light in lightsInSight)
            {
                int range = 0;
                // The specific range of the lights.
                switch (light.transform.parent.tag)
                {
                    case "Sonar":
                        range = 5;
                        break;
                    case "Bullet":
                        range = 1;
                        break;
                    case "Impact":
                        range = 2;
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
        private List<Light> GetLights()
        {
            // Get's all lights in the scene 
            Light[] lights = FindObjectsOfType<Light>();
            // Filter lights by point light and check if the position is inside the view
            List<Light> pointLights = lights.Where(light => light.type == UnityEngine.LightType.Point && CheckIfPositionIsInsideView(light.transform.position)).ToList();
            // Remove point lights of the players and bots
            return pointLights.Where(light => light.transform.parent.tag != "Player").ToList();
        }

        /// <summary>
        /// Checks if a position is inside the view of the bot
        /// </summary>
        /// <param name="position">The position you want to check</param>
        private bool CheckIfPositionIsInsideView(Vector3 position)
        {
            float viewLeft = m_bot.transform.position.x - 11;
            float viewUp = m_bot.transform.position.z + 5;
            float viewRight = viewLeft + 22;
            float viewDown = viewUp - 10;
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
}
