﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum Tile { Unknown = -1, Floor, Wall, BreakableWall, Reflector }

public class Map
{
    public static Vector2Int[] Directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    public Tile[][] TileMap;
    public Vector2Int Pos;

    private int m_spawnAreaMinSize;
    private int m_preferredDistanceFromEnemies = 15;

    public Map(Tile[][] tileMap, int spawnAreaMinSize)
    {
        TileMap = tileMap;
        m_spawnAreaMinSize = spawnAreaMinSize;
    }

    /// <summary>
    /// Generates a map based on the content
    /// </summary>
    /// <param name="content">The tile to fill the map with</param>
    /// <returns>A jagged array of Tiles representing the tilemap</returns>
    static public Map GenerateEmptyMap(Tile content, int width, int height, int spawnAreaMinSize)
    {
        Map result = new Map(new Tile[width][], spawnAreaMinSize);
        for (int x = 0; x < width; x++)
        {
            result.TileMap[x] = new Tile[height];
            for (int y = 0; y < height; y++)
            {
                result.TileMap[x][y] = content;
            }
        }
        return result;
    }

    /// <summary>
    /// Add a tile map to this map
    /// </summary>
    /// <param name="pos">Position of where the small map should be pasted within the big map</param>
    /// <param name="smallMap">Small tile map to be added</param>
    public void PasteTileMap(Vector2Int pos, Tile[][] smallMap)
    {
        // for every tile in the small map
        for (int x = 0; x < smallMap.Length; x++)
        {
            for (int y = 0; y < smallMap[0].Length; y++)
            {
                // change the according tile on the big map
                if (smallMap[x][y] != Tile.Wall)
                {
                    TileMap[x + pos.x][y + pos.y] = smallMap[x][y];
                }
            }
        }
    }

    /// <summary>
    /// Checks if a rectangle between from and to is inside the tilemap bounds
    /// </summary>
    /// <param name="from">Rectangle from</param>
    /// <param name="to">Rectangle to</param>
    /// <returns>Returns true if valid rectangle within tilemap</returns>
    public bool CheckOutOfBounds(Vector2Int from, Vector2Int to)
    {
        if ((from.x > 0 && from.x <= TileMap.Length - 1 && from.y > 0 && from.y <= TileMap[0].Length - 1) && (to.x > 0 && to.x <= TileMap.Length - 1 && to.y > 0 && to.y <= TileMap[0].Length - 1))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a random floor tile from the tilemap
    /// </summary>
    /// <returns>Vector2Int containing the position of the floor tile</returns>
    public Vector2Int GetRandomFloorTile()
    {
        List<Vector2Int> list = GetAllFloorTiles();
        if (list.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, list.Count);
            return list[r];
        }

        return new Vector2Int(-1, -1);
    }

    public Vector2Int GetSpawnPoint(Team team)
    {
        // get all floor tiles and shuffle them
        List<Vector2Int> floorTiles = GetAllFloorTiles();
        Shuffle(floorTiles);
        // init list to be used when no floor tile is the right distance away from enemies
        List<KeyValuePair<Vector2Int, float>> possiblePositions = new List<KeyValuePair<Vector2Int, float>>();

        for (int i = 0; i < floorTiles.Count; i++)
        {
            bool placeable = true;
            float currentTileMinDistance = float.PositiveInfinity;

            List<Soldier> enemies = TeamManager.GetAliveEnemiesByTeam(team.Id);
            for (int j = 0; j < enemies.Count; j++)
            {
                // check if enough distance from enemy
                float distance = Vector3.Distance(enemies[j].transform.position, new Vector3(floorTiles[i].x, 0, floorTiles[j].y));
                if (distance < m_preferredDistanceFromEnemies)
                {
                    placeable = false;
                    // set the minimum possible distance
                    if (distance < currentTileMinDistance)
                    {
                        currentTileMinDistance = distance;
                    }
                }
            }

            if (ValidSpawnPoint(floorTiles[i]))
            {
                if (placeable)
                {
                    return floorTiles[i];
                }
                else
                {
                    possiblePositions.Add(new KeyValuePair<Vector2Int, float>(floorTiles[i], currentTileMinDistance));
                }
            }
        }

        // sort possible positions
        possiblePositions = possiblePositions.OrderByDescending(o => o.Value).ToList();
        for (int i = 0; i < possiblePositions.Count(); i++)
        {
            if (ValidSpawnPoint(possiblePositions[i].Key))
            {
                return possiblePositions[i].Key;
            }
        }

        // returns -1,-1 if no valid floor tile was found
        return new Vector2Int(-1,-1);
    }

    private List<Vector2Int> GetAllFloorTiles()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = 0; i < TileMap.Length; i++)
        {
            for (int j = 0; j < TileMap[0].Length; j++)
            {
                if (TileMap[i][j] == Tile.Floor)
                {
                    list.Add(new Vector2Int(i, j));
                }
            }
        }
        return list;
    }

    private bool ValidSpawnPoint(Vector2Int pos)
    {
        // setup
        Queue<Vector2Int> positionsToCheck = new Queue<Vector2Int>();
        positionsToCheck.Enqueue(pos);
        List<Vector2Int> positionsChecked = new List<Vector2Int>();

        // check if there is enough space using floodfill algorithm
        while (positionsToCheck.Count != 0)
        {
            if (positionsChecked.Count > m_spawnAreaMinSize)
            {
                return true;
            }
            Vector2Int current = positionsToCheck.Dequeue();
            for (int i = 0; i < Directions.Length; i++)
            {
                Vector2Int nextToCurrent = current + Directions[i];

                // if the tile hasn't been checked and is not in the list to be checked, and it's a floor, add to check list
                if (!positionsChecked.Contains(nextToCurrent) && !positionsToCheck.Contains(nextToCurrent)
                    && TileMap[nextToCurrent.x][nextToCurrent.y] == Tile.Floor)
                {
                    positionsToCheck.Enqueue(nextToCurrent);
                }
            }

            positionsChecked.Add(current);
        }

        return false;
    }


    /// <summary>
    /// Adds a tunnel to a given room
    /// </summary>
    /// <param name="tunnelLength">Length of the tunnel</param>
    /// <param name="entrance">The tile within the room that the tunnel will be adjacent to</param>
    /// <param name="direction">Direction of the tunnel</param>
    /// <param name="tunnelWidth">Width of the tunnel</param>
    /// <returns>The new map with the tunnel added to it</returns>
    public Map AddTunnelToMap(int tunnelLength, Vector2Int entrance, Vector2Int direction, int tunnelWidth)
    {
        Map newMap = GenerateEmptyMap(Tile.Wall, TileMap.Length + Math.Abs(direction.x) * tunnelLength, TileMap[0].Length + Math.Abs(direction.y) * tunnelLength, m_spawnAreaMinSize);

        // copy roommap into new map
        newMap.PasteTileMap(new Vector2Int(Math.Abs(Math.Max(0, direction.x)) * tunnelLength, Math.Abs(Math.Max(0, direction.y)) * tunnelLength), TileMap);

        // generate tunnel
        Map tunnel = new Map(new Tile[(tunnelWidth * Math.Abs(direction.y) + tunnelLength * Math.Abs(direction.x))][], m_spawnAreaMinSize);

        for (int x = 0; x < tunnel.TileMap.Length; x++)
        {
            tunnel.TileMap[x] = new Tile[(tunnelWidth * Math.Abs(direction.x) + tunnelLength * Math.Abs(direction.y))];
        }

        // generate position within new map
        tunnel.Pos = new Vector2Int(entrance.x + Math.Abs(Math.Max(0, direction.x)) * tunnelLength, entrance.y + Math.Abs(Math.Max(0, direction.y)) * tunnelLength);
        if (direction == Vector2Int.right)
        {
            tunnel.Pos.x -= tunnelLength;
        }
        else if (direction == Vector2Int.left)
        {
            tunnel.Pos.x += 1;
        }
        else if (direction == Vector2Int.up)
        {
            tunnel.Pos.y -= tunnelLength;
        }
        else if (direction == Vector2Int.down)
        {
            tunnel.Pos.y += 1;
        }

        // add tunnel to new map
        newMap.PasteTileMap(tunnel.Pos, tunnel.TileMap);

        // return new map
        return newMap;
    }



    /// <summary>
    /// Gets a random wall tile based on a direction
    /// </summary>
    /// <param name="direction">Direction the tunnel will face if added to the wall</param>
    /// <param name="tunnelWidth">Width of a tunnel, to make sure a tunnel isn't half connected when it is added to the wall tile</param>
    /// <returns>A Vector2Int containing the position of the wall, returns -1,-1 if no walls were found in the given direction with the given tunnel width</returns>
    public Vector2Int GetRandomWallTile(Vector2Int direction, int tunnelWidth)
    {
        List<Vector2Int> list = GetAllWallTilesInDirection(direction, 1, tunnelWidth).ToList();
        if (list.Count > 0)
        {
            int r = UnityEngine.Random.Range(0, list.Count);
            return list[r];
        }

        return new Vector2Int(-1, -1);
    }

    /// <summary>
    /// Gets a list of all the wall tiles in the map, and what their direction is
    /// </summary>
    /// <param name="outerWidth">The distance between the edge of the map and the area that should be searched</param>
    /// <param name="tunnelWidth">Width of a tunnel, to make sure a tunnel isn't half connected when it is added to the wall tile</param>
    /// <returns>List containing all the wall tiles with direction they face</returns>
    public List<KeyValuePair<Vector2Int, Vector2Int>> GetAllWallTiles(int outerWidth, int tunnelWidth)
    {
        List<KeyValuePair<Vector2Int, Vector2Int>> result = new List<KeyValuePair<Vector2Int, Vector2Int>>();

        for (int i = 0; i < Directions.Length; i++)
        {
            Vector2Int direction = Directions[i];
            List<Vector2Int> list = GetAllWallTilesInDirection(direction, outerWidth, tunnelWidth).ToList();
            for (int j = 0; j < list.Count; j++)
            {
                result.Add(new KeyValuePair<Vector2Int, Vector2Int>(list[j], direction));
            }
        }

        Shuffle(result);
        return result;
    }

    /// <summary>
    /// Checks if the given tile is a wall in the given Direction
    /// </summary>
    /// <param name="randomTile">Tile to be checked</param>
    /// <param name="direction">Direction the tunnel will face if added to the wall</param>
    /// <param name="tunnelWidth">Width of the tunnel to check if a tunnel connected to this tile won't be half connected</param>
    /// <returns>True if tile is indeed a wall based on the direction</returns>
    public bool CheckWallTile(Vector2Int randomTile, Vector2Int direction, int tunnelWidth)
    {
        // check if the surrounding tiles are suitable for the width of the tunnel
        for (int i = 0; i < tunnelWidth; i++)
        {
            Vector2Int wallTile = new Vector2Int(randomTile.x + i * Math.Abs(direction.y), randomTile.y + i * Math.Abs(direction.x));
            if (!(TileMap[wallTile.x][wallTile.y] == Tile.Wall
                    && TileMap[wallTile.x - direction.x][wallTile.y - direction.y] == Tile.Floor))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets a list of all the wall tiles in a certain direction
    /// </summary>
    /// <param name="direction">Direction that should be searched</param>
    /// <param name="outerWidth">The distance between the edge of the map and the area that should be searched</param>
    /// <param name="tunnelWidth">Width of a tunnel, to make sure a tunnel isn't half connected when it is added to the wall tile</param>
    /// <returns></returns>
    private IEnumerable<Vector2Int> GetAllWallTilesInDirection(Vector2Int direction, int outerWidth, int tunnelWidth)
    {
        for (int i = outerWidth; i < TileMap.Length - tunnelWidth - outerWidth; i++)
        {
            for (int j = outerWidth; j < TileMap[0].Length - tunnelWidth - outerWidth; j++)
            {
                Vector2Int tile = new Vector2Int(i, j);
                if (CheckWallTile(tile, direction, tunnelWidth))
                {
                    yield return (tile);
                }
            }
        }
    }

    /// <summary>
    /// Shuffles a list using the Fisher-Yates shuffle
    /// </summary>
    /// <param name="list">List to be shuffled</param>
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void UpdateMap(Vector2Int coordinates)
    {
        TileMap[coordinates.x][coordinates.y] = Tile.Floor;
    }

#if (UNITY_EDITOR)
    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Prints the map given
    /// </summary>
    public void DebugMap()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = TileMap[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < TileMap.Length; x++)
            {
                builder.Append((int)TileMap[x][y]);
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }

    /// <summary>
    /// Prints the adding of a roommap to a map
    /// </summary>
    /// <param name="map">Room to add to the map</param>
    public void DebugAddingRoom(Map map)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = TileMap[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < TileMap.Length; x++)
            {
                if (x >= map.Pos.x && x < map.Pos.x + map.TileMap.Length && y >= map.Pos.y && y < map.Pos.y + map.TileMap[0].Length)
                {
                    builder.Append((int)map.TileMap[x - map.Pos.x][y - map.Pos.y]);
                }
                else
                {
                    builder.Append((int)TileMap[x][y]);
                }
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }
#endif
}
