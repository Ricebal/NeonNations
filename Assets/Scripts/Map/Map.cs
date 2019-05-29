using System;
using System.Text;
using UnityEngine;

public enum Tile { Unknown = -1, Floor, Wall, BreakableWall, Reflector }

public class Map
{
    public Tile[][] TileMap;
    public Vector2Int Pos;

    public Map(Tile[][] tileMap)
    {
        TileMap = tileMap;
    }

    /// <summary>
    /// Generates a map based on the content
    /// </summary>
    /// <param name="content">The tile to fill the map with</param>
    /// <returns>A jagged array of Tiles representing the tilemap</returns>
    static public Map GenerateEmptyMap(Tile content, int width, int height)
    {
        Map result = new Map(new Tile[width][]);
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
    static public void PasteTileMap(Vector2Int pos, Tile[][] smallMap, Tile[][] bigMap)
    {
        // for every tile in the small map
        for (int x = 0; x < smallMap.Length; x++)
        {
            for (int y = 0; y < smallMap[0].Length; y++)
            {
                // change the according tile on the big map
                if (smallMap[x][y] != Tile.Wall)
                {
                    bigMap[x + pos.x][y + pos.y] = smallMap[x][y];
                }
            }
        }
    }

    public bool CheckOutOfBounds(Vector2Int from, Vector2Int to)
    {
        if ((from.x > 0 && from.x <= TileMap.Length - 1 && from.y > 0 && from.y <= TileMap[0].Length - 1) && (to.x > 0 && to.x <= TileMap.Length - 1 && to.y > 0 && to.y <= TileMap[0].Length - 1))
        {
            return false;
        }

        return true;
    }
    
    public Vector2Int GetRandomFloorTile()
    {
        Vector2Int randomTile;
        do
        {
            // get a random tile in the map
            randomTile = new Vector2Int(UnityEngine.Random.Range(1, TileMap.Length - 1), UnityEngine.Random.Range(1, TileMap[0].Length - 1));
        } while (TileMap[randomTile.x][randomTile.y] != Tile.Floor);
        return randomTile;
    }


    /// <summary>
    /// Adds a tunnel to a given room
    /// </summary>
    /// <param name="map">Map the tunnel should be added to</param>
    /// <param name="tunnelLength">Length of the tunnel</param>
    /// <param name="entrance">The tile within the room that the tunnel will be adjacent to</param>
    /// <param name="direction">Direction of the tunnel</param>
    /// <returns>The new map with the tunnel added to it</returns>
    public Map AddTunnelToMap(int tunnelLength, Vector2Int entrance, Vector2Int direction, int tunnelWidth)
    {
        Map newMap = GenerateEmptyMap(Tile.Wall, TileMap.Length + Math.Abs(direction.x) * tunnelLength, TileMap[0].Length + Math.Abs(direction.y) * tunnelLength);

        // copy roommap into new map
        PasteTileMap(new Vector2Int(Math.Abs(Math.Max(0, direction.x)) * tunnelLength, Math.Abs(Math.Max(0, direction.y)) * tunnelLength), TileMap, newMap.TileMap);

        // generate tunnel
        Tile[][] tunnel = new Tile[(tunnelWidth * Math.Abs(direction.y) + tunnelLength * Math.Abs(direction.x))][];

        for (int x = 0; x < tunnel.Length; x++)
        {
            tunnel[x] = new Tile[(tunnelWidth * Math.Abs(direction.x) + tunnelLength * Math.Abs(direction.y))];
        }

        // generate position within new map
        Vector2Int pos = new Vector2Int(entrance.x + Math.Abs(Math.Max(0, direction.x)) * tunnelLength, entrance.y + Math.Abs(Math.Max(0, direction.y)) * tunnelLength);
        if (direction == Vector2Int.right)
        {
            pos.x -= tunnelLength;
        }
        else if (direction == Vector2Int.left)
        {
            pos.x += 1;
        }
        else if (direction == Vector2Int.up)
        {
            pos.y -= tunnelLength;
        }
        else if (direction == Vector2Int.down)
        {
            pos.y += 1;
        }

        // add tunnel to new map
        PasteTileMap(pos, tunnel, newMap.TileMap);

        // return new map
        return newMap;
    }

    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    /// <summary>
    /// Prints the map given
    /// </summary>
    /// <param name="map">Map to print</param>
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
    /// <param name="map">Map to print</param>
    /// <param name="roomMap">Room to add to the map</param>
    /// <param name="pos">Position of the room within the map</param>
    public void DebugAddingRoom(Tile[][] roomMap, Vector2Int pos)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = TileMap[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < TileMap.Length; x++)
            {
                if (x >= pos.x && x < pos.x + roomMap.Length && y >= pos.y && y < pos.y + roomMap[0].Length)
                {
                    builder.Append((int)roomMap[x - pos.x][y - pos.y]);
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
}
