using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BoardManager : MonoBehaviour
{
    public int MapWidth = 30;
    public int MapHeight = 30;
    public int OuterWallWidth = 14;
    public GameObject Map;

    public int RoomAmount = 15;
    public int MaxRoomSize = 80;
    public int MinRoomSize = 36;
    public int MinRoomLength = 6;
    public int MaxPlaceRoomAttempts = 10;
    public int MaxBuildRoomAttempts = 250;
    public int MinTunnelLength = 1;
    public int MaxTunnelLength = 7;
    public int TunnelWidth = 2;

    private int[,] m_tileMap;
    private GameObject m_map;

    // --------------------------------------------------------------------------------------------
    // Public functions
    // --------------------------------------------------------------------------------------------

    public void SetupScene()
    {
        GenerateRandomMap();
        LoadFloor();
        LoadMap();
        CreateOuterWalls();
        CombineMeshes();
    }

    // --------------------------------------------------------------------------------------------
    // Generate maps
    // --------------------------------------------------------------------------------------------

    private void GenerateEmptyMap(int content) {
        m_tileMap = new int[MapWidth, MapHeight];
        for (int x = 0; x < MapWidth; x++) {
            for (int y = 0; y < MapHeight; y++) {
                m_tileMap[x, y] = content;
            }
        }
    }

    private void GenerateTestMap() {
        GenerateEmptyMap(1);

        for (int x = 0; x < MapWidth; x++) {
            for (int y = 0; y < MapHeight; y++) {
                if (// rooms
                    (x == 0 && y == 0
                    || x >= 0 && x <= 9 && y >= 16 && y <= 21
                    || x >= 1 && x <= 7 && y >= 0 && y <= 5
                    || x >= 11 && x <= 15 && y >= 4 && y <= 10
                    || x >= 16 && x <= 20 && y >= 13 && y <= 18
                    || x >= 23 && x <= 29 && y >= 3 && y <= 10
                    || x >= 24 && x <= 27 && y >= 17 && y <= 21
                    // corridors
                    || x >= 1 && x <= 2 && y >= 12 && y <= 15
                    || x >= 3 && x <= 6 && y >= 12 && y <= 13
                    || x >= 5 && x <= 6 && y >= 6 && y <= 13
                    || x >= 7 && x <= 10 && y >= 8 && y <= 9
                    || x >= 8 && x <= 24 && y >= 0 && y <= 1
                    || x >= 10 && x <= 15 && y >= 17 && y <= 18
                    || x >= 13 && x <= 14 && y >= 2 && y <= 3
                    || x >= 16 && x <= 22 && y >= 8 && y <= 9
                    || x >= 17 && x <= 18 && y >= 19 && y <= 21
                    || x >= 17 && x <= 18 && y >= 10 && y <= 12
                    || x >= 19 && x <= 25 && y >= 20 && y <= 21
                    || x >= 23 && x <= 24 && y == 2
                    || x >= 25 && x <= 26 && y >= 11 && y <= 16))
                {
                    m_tileMap[x, y] = 0;
                }
            }
        }
    }

    private void GenerateRandomMap() {
        // create level with only walls
        GenerateEmptyMap(1);

        // generate first room (in the middle)
        Room room = GenerateRandomRoom();
        room.Pos = new Vector2(MapWidth/2 - room.Roommap.GetLength(0)/2, MapHeight / 2 - room.Roommap.GetLength(1)/2);
        // add first room to map
        AddRoom(room);

        int currentRoomAmount = 1;

        // for this amount (starts from 1 since main room has been created)
        for (int i = 0; i < MaxBuildRoomAttempts; i++) {
            // if generate room tries doesn't exceed max buildRoomAttempts
            if (currentRoomAmount >= RoomAmount) {
                break;
            }
            // generate a room
            room = GenerateRandomRoom();
            // try to place the room
            bool placed = PlaceRoom(room);
            // if succeeded
            if (placed) {
                currentRoomAmount++;
            }
        }

        DebugMap(m_tileMap);

        // add shortcuts
    }

    // --------------------------------------------------------------------------------------------
    // Random generation functions
    // --------------------------------------------------------------------------------------------

    Room GenerateRandomRoom() {
        int width = UnityEngine.Random.Range(MinRoomLength, MaxRoomSize/MinRoomLength);
        int height = UnityEngine.Random.Range(MinRoomLength, MaxRoomSize/width);

        int[,] roomMap = new int[width, height];
        return new Room(roomMap);
    }

    void AddRoom(Room room) {
        PasteTileMap(room.Roommap, m_tileMap, room.Pos);
    }

    Vector2 GenerateRandomDirection() {
        return GetDirection(UnityEngine.Random.Range(1, 5)); // max is exclusive, so this will generate number between 1 and 4
    }

    Vector2 GetDirection(int directionInt) {
        Vector2 direction = new Vector2();
        switch (directionInt) {
            case 1:
                direction = new Vector2(0, 1);
                break;
            case 2:
                direction = new Vector2(1, 0);
                break;
            case 3:
                direction = new Vector2(0, -1);
                break;
            case 4:
                direction = new Vector2(-1, 0);
                break;
        }
        return direction;
    }

    Vector2 GetRandomWallTile(Vector2 direction) {
        Vector2 wallTile = new Vector2(-1,-1);
        while (wallTile.x == -1 && wallTile.y == -1) {
            // generate a random tile in the map
            Vector2 randomTile = new Vector2(UnityEngine.Random.Range(1, MapWidth - TunnelWidth), UnityEngine.Random.Range(1, MapHeight - TunnelWidth));

            bool possible = true;

            // check if the surrounding tiles are suitable for the width of the tunnel
            for (int i = 0; i < TunnelWidth; i++) {
                if (direction.x == 0) { // if direction is on the y-axis
                    if (!(m_tileMap[(int)randomTile.x + i, (int)randomTile.y] == 1
                        && m_tileMap[(int)randomTile.x + i + (int)direction.x, (int)randomTile.y + (int)direction.y] == 1
                        && m_tileMap[(int)randomTile.x + i - (int)direction.x, (int)randomTile.y - (int)direction.y] == 0)) {
                        possible = false;
                    }
                }

                if (direction.y == 0) { // if direction is on the x-axis
                    if (!(m_tileMap[(int)randomTile.x, (int)randomTile.y + i] == 1
                        && m_tileMap[(int)randomTile.x + (int)direction.x, (int)randomTile.y + i + (int)direction.y] == 1
                        && m_tileMap[(int)randomTile.x - (int)direction.x, (int)randomTile.y + i - (int)direction.y] == 0)) {
                        possible = false;
                    }
                }
            }

            if (possible) {
                wallTile = randomTile;
            }
        }

        return wallTile;
    }

    void AddTunnel(Vector2 from, Vector2 direction, int length) {
        Vector2 tunnelSize = new Vector2(TunnelWidth, TunnelWidth);
        if (direction.x != 0)
            tunnelSize.x = length;
        else if (direction.y != 0)
            tunnelSize.y = length;
        int[,] tunnel = new int[(int)tunnelSize.x, (int)tunnelSize.y];

        Vector2 pos = new Vector2(Math.Min(from.x, from.x + (direction.x * length)+1) , Math.Min(from.y, from.y + (direction.y * length)+1));
        PasteTileMap(tunnel, m_tileMap, pos);
    }

    void PasteTileMap(int[,] smallMap, int[,] bigMap, Vector2 pos) {
        // for every tile in the room
        for (int x = 0; x < smallMap.GetLength(0); x++) {
            for (int y = 0; y < smallMap.GetLength(1); y++) {
                // change the according tile on the map
                bigMap[x + (int)pos.x, y + (int)pos.y] = smallMap[x, y];
            }
        }
    }

    bool PlaceRoom(Room room) {
        for (int i = 1; i <= MaxPlaceRoomAttempts; i++) {
            room = TryPlacement(room);
            if (room.Pos.x != -1 && room.Pos.y != -1) {
                // add room and tunnel to the map
                AddRoom(room);
                //AddTunnel(room.WallTile, room.Direction, room.TunnelLength);
                return true;
            }
        }
        return false;
    }

    Room TryPlacement (Room room) {
        // get random direction to attach the room to the map
        room.Direction = GenerateRandomDirection();

        // get random walltile for that direction
        room.WallTile = GetRandomWallTile(room.Direction);

        // get a random tile within the room, to attach the corridor to
        room.Entrance = new Vector2(UnityEngine.Random.Range(0, room.Roommap.GetLength(0) + 1 - TunnelWidth), UnityEngine.Random.Range(0, room.Roommap.GetLength(1) + 1 - TunnelWidth));
        if (room.Direction.y == -1) {
            room.Entrance.y = room.Roommap.GetLength(1) - 1;
        } else if (room.Direction.y == 1) {
            room.Entrance.y = 0;
        } else if (room.Direction.x == -1) {
            room.Entrance.x = room.Roommap.GetLength(0) - 1;
        } else if (room.Direction.x == 1) {
            room.Entrance.x = 0;
        }

        // determine the position of the room within the map, based on the walltile and the tile within the room
        room.Pos = new Vector2(room.WallTile.x - room.Entrance.x, room.WallTile.y - room.Entrance.y);

        // generate random ordered list of tunnel lengths
        IEnumerable<int> tunnelLengths = UniqueRandom(MinTunnelLength, MaxTunnelLength);

        foreach(int i in tunnelLengths) {
            // set length of the tunnel to the specific length
            room.TunnelLength = i;

            int[,] tempMap = AddTunnelToMap(room);
            Vector2 tempEntrance = new Vector2(room.Entrance.x, room.Entrance.y);

            // adjust position and entrance of the room based on the length of the tunnel
            if (room.Direction.y == -1)
            {
                room.Pos.y = room.WallTile.y + 1 - tempMap.GetLength(1);
                tempEntrance.y += room.TunnelLength;
            }
            else if (room.Direction.x == -1)
            {
                room.Pos.x = room.WallTile.x + 1 - tempMap.GetLength(0);
                tempEntrance.x += room.TunnelLength;
            }

            // adjust entrance
            for (int j = 0; j < TunnelWidth; j++)
            {
                tempMap[(int)tempEntrance.x + Math.Abs((int)room.Direction.y)*j, (int)tempEntrance.y + Math.Abs((int)room.Direction.x) * j] = 2;
            }

            //DebugMap(tempMap);
            //DebugAddingRoom(m_tileMap, tempMap, room.Pos);

            // if it can be placed, return the room
            if (CanPlace(tempMap, room.Pos)) {
                room.Roommap = tempMap;
                return room;
            }
        }

        // for every length of the tunnel, this room doesn't fit. set the pos to -1,-1 and return
        room.Pos = new Vector2(-1, -1);
        return room;
    }

    int[,] AddTunnelToMap(Room room)
    {
        // create new map with right size
        int[,] newMap = new int[room.Roommap.GetLength(0) + (int)Math.Abs(room.Direction.x) * room.TunnelLength, room.Roommap.GetLength(1) + (int)Math.Abs(room.Direction.y) * room.TunnelLength];
        for (int x = 0; x < newMap.GetLength(0); x++) {
            for (int y = 0; y < newMap.GetLength(1); y++) {
                newMap[x, y] = 1;
            }
        }

        // copy roommap into new map
        for (int x = 0; x < room.Roommap.GetLength(0); x++) {
            for (int y = 0; y < room.Roommap.GetLength(1); y++) {
                newMap[x + (int)Math.Abs(Math.Max(0, room.Direction.x)) * room.TunnelLength, y + (int)Math.Abs(Math.Max(0, room.Direction.y)) * room.TunnelLength] = room.Roommap[x, y];
            }
        }

        // copy tunnel into new map
        // generate tunnel
        // IDEA: (absolute of direction * -1 + 1) * width
        Vector2 tunnelSize = new Vector2(TunnelWidth, TunnelWidth);
        if (room.Direction.x != 0)
            tunnelSize.x = room.TunnelLength;
        else if (room.Direction.y != 0)
            tunnelSize.y = room.TunnelLength;
        int[,] tunnel = new int[(int)tunnelSize.x, (int)tunnelSize.y];

        // generate position within new map
        Vector2 pos = new Vector2(room.Entrance.x + (int)Math.Abs(Math.Max(0, room.Direction.x)) * room.TunnelLength, room.Entrance.y + (int)Math.Abs(Math.Max(0, room.Direction.y)) * room.TunnelLength);
        if (room.Direction.x == 1) {
            pos.x = pos.x - room.TunnelLength;
        } else if (room.Direction.x == -1) {
            pos.x = pos.x + 1;
        } else if (room.Direction.y == 1) {
            pos.y = pos.y - room.TunnelLength;
        } else if (room.Direction.y == -1) {
            pos.y = pos.y + 1;
        }

        // add tunnel to new map
        for (int x = 0; x < tunnel.GetLength(0); x++) {
            for (int y = 0; y < tunnel.GetLength(1); y++) {
                newMap[x + (int)pos.x, y + (int)pos.y] = 0;
            }
        }

        // return new map
        return newMap;
    }

    bool CanPlace(int[,] map, Vector2 pos)
    {
        // check out of bounds
        if (pos.x <= 0 || pos.x > MapWidth - map.GetLength(0) - 1 || pos.y <= 0 || pos.y > MapHeight - map.GetLength(1) - 1) {
            return false;
        }

        // check overlap
        for (int x = 0; x < map.GetLength(0); x++) {
            for (int y = 0; y < map.GetLength(1); y++) {
                if (map[x, y] == 0 && // check if position in room equals zero
                    (m_tileMap[x + (int)pos.x, y + (int)pos.y] == 0 || // if so, check if the same position on map is zero
                    m_tileMap[x + (int)pos.x, y + (int)pos.y + 1] == 0 || // if so, check all tiles around the position on the map is zero, starting with north
                    m_tileMap[x + (int)pos.x + 1, y + (int)pos.y + 1] == 0 || // northeast
                    m_tileMap[x + (int)pos.x + 1, y + (int)pos.y] == 0 || // east
                    m_tileMap[x + (int)pos.x + 1 , y + (int)pos.y - 1] == 0 || // southeast
                    m_tileMap[x + (int)pos.x, y + (int)pos.y - 1] == 0 || // south
                    m_tileMap[x + (int)pos.x - 1, y + (int)pos.y - 1] == 0 || // southwest
                    m_tileMap[x + (int)pos.x - 1, y + (int)pos.y] == 0 || // west
                    m_tileMap[x + (int)pos.x - 1, y + (int)pos.y + 1] == 0 // northwest
                    ))
                    return false;
            }
        }

        return true;
    }

    // --------------------------------------------------------------------------------------------
    // Create map functions
    // --------------------------------------------------------------------------------------------

    void LoadFloor() {
        m_map = Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3((float)MapWidth / 2 - 0.5f, -0.5f, (float)MapHeight / 2 - 0.5f);
        floor.transform.localScale = new Vector3((float)(MapWidth + OuterWallWidth * 2) / 10, 1, (float)(MapHeight + OuterWallWidth * 2) / 10);
        floor.transform.SetParent(m_map.transform);
    }

    void LoadMap() {
        for (int i = 0; i < m_tileMap.GetLength(0); i++) {
            for (int j = 0; j < m_tileMap.GetLength(1); j++) {
                if (m_tileMap[i, j] == 1) { // if 1, build wall
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    void CreateOuterWalls() {
        for (int i = -OuterWallWidth; i < m_tileMap.GetLength(0) + OuterWallWidth; i++) {
            for (int j = -OuterWallWidth; j < m_tileMap.GetLength(1) + OuterWallWidth; j++) {
                if (i < 0 /*&& j != 0 */ || i >= m_tileMap.GetLength(0) ||j < 0 || j >= m_tileMap.GetLength(1)) {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    void CombineMeshes() {
        MeshFilter[] meshFilters = m_map.GetComponentsInChildren<MeshFilter>();
        Mesh finalMesh = new Mesh();
        CombineInstance[] combiners = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++) {
            combiners[i].subMeshIndex = 0;
            combiners[i].mesh = meshFilters[i].sharedMesh;
            combiners[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        finalMesh.CombineMeshes(combiners);
        m_map.transform.GetComponent<MeshFilter>().sharedMesh = finalMesh;
        m_map.transform.gameObject.SetActive(true);

        var collider = m_map.GetComponent<MeshCollider>();
        collider.sharedMesh = finalMesh;

        foreach (Transform child in m_map.transform) {
            Destroy(child.gameObject);
        }
    }

    // --------------------------------------------------------------------------------------------
    // Helper functions
    // --------------------------------------------------------------------------------------------

    private IEnumerable<int> UniqueRandom(int min, int max)
    {
        List<int> candidates = new List<int>();
        for (int i = min; i <= max; i++)
        {
            candidates.Add(i);
        }
        while (candidates.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, candidates.Count);
            yield return candidates[index];
            candidates.RemoveAt(index);
        }
    }

    // --------------------------------------------------------------------------------------------
    // Debug functions
    // --------------------------------------------------------------------------------------------

    private void DebugMap(int[,] map)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map.GetLength(1) - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                builder.Append(map[x, y]);
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }

    private void DebugAddingRoom(int[,] map, int[,] roommap, Vector2 pos)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map.GetLength(1) - 1; y >= 0; y--) {
            for (int x = 0; x < map.GetLength(0); x++) {
                if (x >= pos.x && x < pos.x + roommap.GetLength(0) && y >= pos.y && y < pos.y + roommap.GetLength(1)) {
                    builder.Append(roommap[x - (int)pos.x, y - (int)pos.y] + 2);
                } else {
                    builder.Append(map[x, y]);
                }
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }
}

public class Room
{
    public int[,] Roommap;
    public Vector2 Pos;
    public Vector2 WallTile;
    public Vector2 Direction;
    public int TunnelLength;
    public Vector2 Entrance;

    public Room(int[,] roomMap)
    {
        Roommap = roomMap;
    }
}
