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
        private int[][] m_map;

        public GameEnvironment(GameObject bot)
        {
            m_map = GameObject.Find("GameManager").GetComponent<BoardManager>().GetTileMap();
            m_bot = bot;

        }

        //CheckForCollision();

        /// <summary>
        ///Swap the values of A and B
        /// </summary>
        private void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        /// <summary>
        /// Returns a List of Coordinates which collide with the line between a startpoint and an endpoint
        /// </summary>
        /// <param name="startPointX">The x coordinate of the startpoint</param>
        /// <param name="startPointY">The y coordinate of the startpoint</param>
        /// <param name="endPointX">The x coordinate of the endpoint</param>
        /// <param name="endPointY">The y coordinate of the endpoint</param>
        /// <returns>List of intersecting Coordinates</returns>
        private List<Coordinates> BresenhamLine(int startPointX, int startPointY, int endPointX, int endPointY)
        {
            List<Coordinates> result = new List<Coordinates>();

            // If the differnce in hight of the line is higher than the differnce of width, it will be a steep line.
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
            if (startPointY < endPointY) ystep = 1; else ystep = -1;
            for (int x = startPointX; x <= endPointX; x++)
            {
                if (steep) result.Add(new Coordinates(y, x));
                else result.Add(new Coordinates(x, y));
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }
            return result;
        }

        /// <summary>
        /// Checks if it's possible to travel straight from one note to the other
        /// </summary>
        /// <param name="currentPosition">The start-position on the map to check</param>
        /// <param name="newPosition">The end-position on the map to check</param>
        /// <returns></returns>
        public bool PossibleToMoveBetween(Node currentPosition, Node newPosition)
        {

            return true;
        }

        /// <summary>
        /// Gives the current map
        /// </summary>
        /// <returns>int[][] Map</returns>
        public int[][] GetMap()
        {
            return m_map;
        }

        // Return the obstacles the bot should be able to see
        public LinkedList<Coordinates> GetObstaclesInVision()
        {
            // First get all coordinates that are illuminated
            LinkedList<Coordinates> currentCoordinatesInSight = GetIlluminatedCoordinates();
            // Than check the map-layout to see if the illuminated parts are walls or empty spaces
            FilterCoordinatesWithObstacle(ref currentCoordinatesInSight);
            // Return the result
            return currentCoordinatesInSight;
        }

        /// <summary>
        /// Checks if the Coordinates in sight are walls or empty spaces.
        /// </summary>
        /// <param name="coordinates">The Coordinates that should be checked</param>
        private void FilterCoordinatesWithObstacle(ref LinkedList<Coordinates> coordinates)
        {
            LinkedList<Coordinates> coordinatesToDelete = new LinkedList<Coordinates>();
            foreach (Coordinates coordinate in coordinates)
            {
                // If the coordinates are outside the map they shouldn't be checked (to prevent out of index exception)
                if (coordinate.X < 0 || coordinate.Y < 0 || coordinate.X >= m_map.Length || coordinate.Y >= m_map[0].Length)
                {
                    coordinatesToDelete.AddLast(coordinate);
                    continue;
                }
                // If the coordinates are inside the map, check if the coordinate contains an obstacle
                if (m_map[coordinate.X][coordinate.Y] == 0)
                {
                    coordinatesToDelete.AddLast(coordinate);
                }
            }
            // Delete the coordintes that contain no obstacles
            foreach (Coordinates coordinate in coordinatesToDelete)
            {
                coordinates.Remove(coordinate);
            }
        }

        /// <summary>
        /// Get all Coordinates that are illuminated by light on the bot's screen
        /// </summary>
        private LinkedList<Coordinates> GetIlluminatedCoordinates()
        {
            LinkedList<Coordinates> illuminatedCoordinates = GetCoordinatesAroundBot(); // Because of the spotlight around the bot
            GetCoordinatesAroundLights(ref illuminatedCoordinates); // All other lights visible on screen
            return illuminatedCoordinates;
        }

        /// <summary>
        /// Get the Coordinates that are illuminated by the spotlight on the bot
        /// </summary>
        private LinkedList<Coordinates> GetCoordinatesAroundBot()
        {
            LinkedList<Coordinates> currentCoordinatesInSight = new LinkedList<Coordinates>();
            Coordinates botCoordinate = ConvertGameObjectToCoordinates(m_bot.transform);
            return GetCoordinatesInRange(botCoordinate, 2, currentCoordinatesInSight);
        }

        /// <summary>
        /// Returns a linked list containing all map-coordinates that should be illuminated by the object
        /// </summary>
        /// <param name="objectCoordinates">The Coordinates of the object you want to check the light from</param>
        /// <param name="range">The range of the light that's emitted by the object</param>
        /// <param name="coordinatesInRange">The Coordinates that have to be checked</param>
        private LinkedList<Coordinates> GetCoordinatesInRange(Coordinates objectCoordinates, int range, LinkedList<Coordinates> coordinatesInRange)
        {
            range = Mathf.Clamp(range, 0, 3);
            for (int i = -range; i < range; i++)
            {
                for (int j = -range; j < range; j++)
                {
                    int x = objectCoordinates.X + i;
                    int y = objectCoordinates.Y + j;
                    // Check if coordinate is already added to the list
                    if (!coordinatesInRange.Any(coordinate => coordinate.X == x && coordinate.Y == y))
                    {
                        coordinatesInRange.AddLast(new Coordinates(x, y));
                    }
                }
            }
            return coordinatesInRange;
        }

        /// <summary>
        /// Get coordinates that are illuminated by lights
        /// </summary>
        /// <param name="illuminatedCoordinates">LinkedListist to which the illuminated Coordinates should be added</param>
        private void GetCoordinatesAroundLights(ref LinkedList<Coordinates> illuminatedCoordinates)
        {
            List<Light> lightsInSight = GetLights();

            // Get all illuminated coordinates
            foreach (Light light in lightsInSight)
            {
                GetCoordinatesInRange(ConvertGameObjectToCoordinates(light.transform), (int)Math.Floor(light.range), illuminatedCoordinates);
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
            List<Light> pointLights = lights.Where(light => light.type == UnityEngine.LightType.Point && CheckIfPositionIsInsideView(light.transform.position)/* && light.transform.parent.tag != "Player"*/).ToList();
            // Remove point lights of the players and bots
            for (int i = 0; i < pointLights.Count(); i++)
            {
                string tag = pointLights[i].transform.parent.tag;
                Console.WriteLine(tag);
            }
            return pointLights.Where(light => light.transform.parent.tag != "Player").ToList();
        }

        /// <summary>
        /// Checks if a position is inside the view of the bot
        /// </summary>
        /// <param name="position">The position you want to check</param>
        private bool CheckIfPositionIsInsideView(Vector3 position)
        {
            float viewLeft = m_bot.transform.position.x - 6;
            float viewUp = m_bot.transform.position.z + 12;
            float viewRight = viewLeft + 12;
            float viewDown = viewUp - 24;
            float posX = position.x;
            float posZ = position.z;
            return (viewLeft <= posX && posX <= viewRight && viewDown <= posZ && posZ <= viewUp);
        }

        /// <summary>
        /// Converts the transform of a gameobject to coordinates on the map
        /// </summary>
        /// <param name="transform">The transform of the gameobject from which you want the coordinates</param>
        public Coordinates ConvertGameObjectToCoordinates(Transform transform)
        {
            return new Coordinates((int)Math.Round(transform.position.x, 0, MidpointRounding.AwayFromZero), (int)Math.Round(transform.position.z, 0, MidpointRounding.AwayFromZero));
        }
    }
}
