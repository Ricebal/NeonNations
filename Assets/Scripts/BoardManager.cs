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

    public int MaxRoomAmount = 15;
    public int MaxShortcutAmount = 10;
    public int MaxRoomSize = 80;
    public int MinRoomSize = 36;
    public int MinRoomLength = 6;
    public int MaxPlaceAttempts = 10;
    public int MaxBuildAttempts = 250;
    public int MaxShortcutAttempts = 250;
    public int MinTunnelLength = 1;
    public int MaxTunnelLength = 7;
    public int TunnelWidth = 2;
    
    private int[][] m_tileMap;
    private GameObject m_map;
    private List<GameObject> m_mapParts = new List<GameObject>();

    // --------------------------------------------------------------------------------------------
    // Public functions
    // --------------------------------------------------------------------------------------------

    public void SetupScene()
    {
        GenerateRandomMap();
        LoadFloor();
        LoadMap();
        CreateOuterWalls();
        CombineAllMeshes();
    }

    // --------------------------------------------------------------------------------------------
    // Generate maps
    // --------------------------------------------------------------------------------------------

    private void GenerateEmptyMap(int content)
    {
        m_tileMap = new int[MapWidth][];
        for (int x = 0; x < MapWidth; x++)
        {
            m_tileMap[x] = new int[MapHeight];
            for (int y = 0; y < MapHeight; y++)
            {
                m_tileMap[x][y] = content;
            }
        }
    }

    private void GenerateTestMap()
    {
        GenerateEmptyMap(1);

        for (int x = 0; x < MapWidth; x++)
        {
            for (int y = 0; y < MapHeight; y++)
            {
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
                    m_tileMap[x][y] = 0;
                }
            }
        }
    }

    private void GenerateRandomMap()
    {
        // create level with only walls
        GenerateEmptyMap(1);

        // generate first room (in the middle)
        Room room = GenerateRandomRoom();
        room.Pos = new Vector2(MapWidth / 2 - room.Roommap.Length / 2, MapHeight / 2 - room.Roommap[0].Length / 2);
        // add first room to map
        AddRoom(room);

        int currentRoomAmount = 1;
        int currentBuildAttempt = 0;

        while (currentBuildAttempt < MaxBuildAttempts && currentRoomAmount < MaxRoomAmount)
        {
            // generate a room
            room = GenerateRandomRoom();
            // try to place the room
            bool placed = PlaceRoom(room);
            // if succeeded
            if (placed)
            {
                currentRoomAmount++;
            }
            currentBuildAttempt++;
        }

        DebugMap(m_tileMap);

        // add shortcuts
        AddShortcuts();
        DebugMap(m_tileMap);
    }

    // --------------------------------------------------------------------------------------------
    // Random generation functions
    // --------------------------------------------------------------------------------------------

    private Room GenerateRandomRoom()
    {
        int width = UnityEngine.Random.Range(MinRoomLength, MaxRoomSize / MinRoomLength);
        int height = UnityEngine.Random.Range(MinRoomLength, MaxRoomSize / width);

        int[][] roomMap = new int[width][];
        for (int x = 0; x < width; x++)
        {
            roomMap[x] = new int[height];
        }
        return new Room(roomMap);
    }

    private void AddRoom(Room room)
    {
        PasteTileMap(room.Roommap, m_tileMap, room.Pos);
    }

    private void PasteTileMap(int[][] smallMap, int[][] bigMap, Vector2 pos)
    {
        // for every tile in the room
        for (int x = 0; x < smallMap.Length; x++)
        {
            for (int y = 0; y < smallMap[0].Length; y++)
            {
                // change the according tile on the map
                bigMap[x + (int)pos.x][y + (int)pos.y] = smallMap[x][y];
            }
        }
    }

    private bool PlaceRoom(Room room)
    {
        for (int i = 1; i <= MaxPlaceAttempts; i++)
        {
            room = TryPlacement(room);
            if (room.Pos.x != -1 && room.Pos.y != -1)
            {
                // add room to the map
                AddRoom(room);
                return true;
            }
        }
        return false;
    }

    private Room TryPlacement(Room room)
    {
        // get random direction to attach the room to the map
        Vector2 direction = GenerateRandomDirection();

        // get random walltile for that direction
        Vector2 wallTile = GetRandomWallTile(direction);

        // get a random tile within the room, to attach the corridor to
        Vector2 entrance = new Vector2(UnityEngine.Random.Range(0, room.Roommap.Length + 1 - TunnelWidth), UnityEngine.Random.Range(0, room.Roommap[0].Length + 1 - TunnelWidth));
        if (direction.y == -1)
        {
            entrance.y = room.Roommap[0].Length - 1;
        }
        else if (direction.y == 1)
        {
            entrance.y = 0;
        }
        else if (direction.x == -1)
        {
            entrance.x = room.Roommap.Length - 1;
        }
        else if (direction.x == 1)
        {
            entrance.x = 0;
        }

        // determine the position of the room within the map, based on the walltile and the tile within the room
        room.Pos = new Vector2(wallTile.x - entrance.x, wallTile.y - entrance.y);

        // generate random ordered list of tunnel lengths
        IEnumerable<int> tunnelLengths = UniqueRandom(MinTunnelLength, MaxTunnelLength);

        foreach (int i in tunnelLengths)
        {
            // create temporary values for this length
            int[][] tempMap = AddTunnelToMap(room, i, entrance, direction);
            Vector2 tempEntrance = new Vector2(entrance.x, entrance.y);

            // adjust position and entrance of the room based on the length of the tunnel
            if (direction.y == -1)
            {
                room.Pos.y = wallTile.y + 1 - tempMap[0].Length;
                tempEntrance.y += i;
            }
            else if (direction.x == -1)
            {
                room.Pos.x = wallTile.x + 1 - tempMap.Length;
                tempEntrance.x += i;
            }

            // calculate all entrance tiles
            List<Vector2> entranceTiles = new List<Vector2>();
            for (int j = 0; j < TunnelWidth; j++)
            {
                entranceTiles.Add(new Vector2((int)tempEntrance.x + Math.Abs((int)direction.y) * j, (int)tempEntrance.y + Math.Abs((int)direction.x) * j));
            }

            // if it can be placed, return the room
            if (CanPlace(tempMap, room.Pos, entranceTiles, direction))
            {
                room.Roommap = tempMap;
                return room;
            }
        }

        // for every length of the tunnel, this room doesn't fit. set the pos to -1,-1 and return
        room.Pos = new Vector2(-1, -1);
        return room;
    }

    private Vector2 GenerateRandomDirection()
    {
        return GetDirection(UnityEngine.Random.Range(1, 5)); // max is exclusive, so this will generate number between 1 and 4
    }

    private Vector2 GetDirection(int directionInt)
    {
        Vector2 direction = new Vector2();
        switch (directionInt)
        {
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

    private Vector2 GetRandomWallTile(Vector2 direction)
    {
        while (true)
        {
            // generate a random tile in the map
            Vector2 randomTile = new Vector2(UnityEngine.Random.Range(1, MapWidth - TunnelWidth), UnityEngine.Random.Range(1, MapHeight - TunnelWidth));

            if (CheckWallTile(randomTile, direction))
            {
                return randomTile;
            }
        }
    }

    private bool CheckWallTile(Vector2 randomTile, Vector2 direction)
    {
        // check if the surrounding tiles are suitable for the width of the tunnel
        for (int i = 0; i < TunnelWidth; i++)
        {
            if (!(m_tileMap[(int)randomTile.x + i * (int)Math.Abs(direction.y)][(int)randomTile.y + i * (int)Math.Abs(direction.x)] == 1
                    && m_tileMap[(int)randomTile.x + i * (int)Math.Abs(direction.y) + (int)direction.x][(int)randomTile.y + i * (int)Math.Abs(direction.x) + (int)direction.y] == 1
                    && m_tileMap[(int)randomTile.x + i * (int)Math.Abs(direction.y) - (int)direction.x][(int)randomTile.y + i * (int)Math.Abs(direction.x) - (int)direction.y] == 0))
            {
                return false;
            }
        }
        return true;
    }

    private int[][] AddTunnelToMap(Room room, int tunnellength, Vector2 entrance, Vector2 direction)
    {
        // create new map with right size
        int[][] newMap = new int[room.Roommap.Length + (int)Math.Abs(direction.x) * tunnellength][];
        for (int x = 0; x < newMap.Length; x++)
        {
            newMap[x] = new int[room.Roommap[0].Length + (int)Math.Abs(direction.y) * tunnellength];
            for (int y = 0; y < newMap[0].Length; y++)
            {
                newMap[x][y] = 1;
            }
        }

        // copy roommap into new map
        PasteTileMap(room.Roommap, newMap, new Vector2((int)Math.Abs(Math.Max(0, direction.x)) * tunnellength, (int)Math.Abs(Math.Max(0, direction.y)) * tunnellength));

        // generate tunnel
        int[][] tunnel = new int[(int)(TunnelWidth * Math.Abs(direction.y) + tunnellength * Math.Abs(direction.x))][];

        for (int x = 0; x < tunnel.Length; x++)
        {
            tunnel[x] = new int[(int)(TunnelWidth * Math.Abs(direction.x) + tunnellength * Math.Abs(direction.y))];
        }

        // generate position within new map
        Vector2 pos = new Vector2(entrance.x + (int)Math.Abs(Math.Max(0, direction.x)) * tunnellength, entrance.y + (int)Math.Abs(Math.Max(0, direction.y)) * tunnellength);
        if (direction.x == 1)
        {
            pos.x -= tunnellength;
        }
        else if (direction.x == -1)
        {
            pos.x += 1;
        }
        else if (direction.y == 1)
        {
            pos.y -= tunnellength;
        }
        else if (direction.y == -1)
        {
            pos.y += 1;
        }

        // add tunnel to new map
        PasteTileMap(tunnel, newMap, pos);

        // return new map
        return newMap;
    }

    private bool CanPlace(int[][] map, Vector2 pos, List<Vector2> entranceTiles, Vector2 direction)
    {
        // check out of bounds
        if (pos.x <= 0 || pos.x > MapWidth - map.Length - 1 || pos.y <= 0 || pos.y > MapHeight - map[0].Length - 1)
        {
            return false;
        }

        // check overlap
        for (int x = 0; x < map.Length; x++)
        {
            for (int y = 0; y < map[0].Length; y++)
            {
                if (map[x][y] == 0 && // check if position in room equals zero
                    !entranceTiles.Contains(new Vector2(x,y)) && // check if the tile isn't an entrance
                    (m_tileMap[x + (int)pos.x][y + (int)pos.y] == 0 || // if so, check if the same position on map is zero
                    m_tileMap[x + (int)pos.x][y + (int)pos.y + 1] == 0 || // and check all tiles around the position on the map is zero, starting with north
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y + 1] == 0 || // northeast
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y] == 0 || // east
                    m_tileMap[x + (int)pos.x + 1][y + (int)pos.y - 1] == 0 || // southeast
                    m_tileMap[x + (int)pos.x][y + (int)pos.y - 1] == 0 || // south
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y - 1] == 0 || // southwest
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y] == 0 || // west
                    m_tileMap[x + (int)pos.x - 1][y + (int)pos.y + 1] == 0 // northwest
                    ))
                    return false;
            }
        }
        return true;
    }

    private void AddShortcuts()
    {
        int currentShortcutAttempt = 0;
        int currentShortcutAmount = 0;
        while (currentShortcutAttempt < MaxShortcutAttempts && currentShortcutAmount < MaxShortcutAmount)
            {
            // get random direction to create a shortcut
            Vector2 direction = GenerateRandomDirection();

            // get random walltile for that direction
            Vector2 wallTile = GetRandomWallTile(direction);
            Vector2 otherWallTile = Vector2.zero;

            for (int j = MinTunnelLength; j < MaxTunnelLength; j++)
            {
                otherWallTile = new Vector2(wallTile.x + direction.x * (j-1), (int)(wallTile.y + direction.y * (j-1)));
                if (!(otherWallTile.x <= 1 || otherWallTile.x >= MapWidth - 1 || otherWallTile.y <= 1 || otherWallTile.y >= MapHeight - 1) && CheckWallTile(otherWallTile, new Vector2(direction.x*-1, direction.y*-1)))
                {
                    // generate tunnel
                    int[][] tunnel = new int[(int)(TunnelWidth * Math.Abs(direction.y) + j * Math.Abs(direction.x))][];

                    for (int x = 0; x < tunnel.Length; x++)
                    {
                        tunnel[x] = new int[(int)(TunnelWidth * Math.Abs(direction.x) + j * Math.Abs(direction.y))];
                    }

                    // generate position of tunnel
                    Vector2 pos = new Vector2();
                    if (direction.x > 0 || direction.y > 0)
                    {
                        pos = wallTile;
                    }
                    else
                    {
                        pos = otherWallTile;
                    }

                    // set entrances
                    List<Vector2> entranceTiles = new List<Vector2>();
                    for (int k = 0; k < TunnelWidth; k++)
                    {
                        entranceTiles.Add(new Vector2(direction.y*k, direction.x*k));
                        // TODO check this
                        entranceTiles.Add(new Vector2(direction.y * k + direction.x * (tunnel[0].Length - 1), direction.y * (tunnel[0].Length - 1) + direction.x * k));
                    }

                    // check if placeable
                    if (CanPlace(tunnel, pos, entranceTiles, direction)) {
                        PasteTileMap(tunnel, m_tileMap, pos);
                        currentShortcutAmount++;
                    }
                }
            }
            currentShortcutAttempt++;
        }
    }

    // --------------------------------------------------------------------------------------------
    // Create map functions
    // --------------------------------------------------------------------------------------------

    private void LoadFloor()
    {
        m_map = new GameObject();
        m_map.name = "Map";
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3((float)MapWidth / 2 - 0.5f, -0.5f, (float)MapHeight / 2 - 0.5f);
        floor.transform.localScale = new Vector3((float)(MapWidth + OuterWallWidth * 2) / 10, 1, (float)(MapHeight + OuterWallWidth * 2) / 10);
        floor.transform.SetParent(m_map.transform);
    }

    private void LoadMap()
    {
        for (int i = 0; i < m_tileMap.Length; i++)
        {
            for (int j = 0; j < m_tileMap[0].Length; j++)
            {
                if (m_tileMap[i][j] == 1)
                { // if 1, build wall
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    // calculate walls
                    Walls walls = Walls.None;
                    if (j + 1 < m_tileMap[0].Length && m_tileMap[i][j + 1] == 0)
                    {
                        walls = walls | Walls.Up;
                    }
                    if (i + 1 < m_tileMap.Length && m_tileMap[i + 1][j] == 0)
                    {
                        walls = walls | Walls.Right;
                    }
                    if (j - 1 >= 0 && m_tileMap[i][j - 1] == 0)
                    {
                        walls = walls | Walls.Down;
                    }
                    if (i - 1 >= 0 && m_tileMap[i - 1][j] == 0)
                    {
                        walls = walls | Walls.Left;
                    }

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = generateNewMesh(walls);

                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    private void CreateOuterWalls()
    {
        for (int i = -OuterWallWidth; i < m_tileMap.Length + OuterWallWidth; i++)
        {
            for (int j = -OuterWallWidth; j < m_tileMap[0].Length + OuterWallWidth; j++)
            {
                if (i < 0 || i >= m_tileMap.Length || j < 0 || j >= m_tileMap[0].Length)
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    //Walls walls = Walls.None;

                    instance.transform.GetComponent<MeshFilter>().sharedMesh = generateNewMesh(Walls.None);

                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(m_map.transform);
                }
            }
        }
    }

    private Mesh generateNewMesh(Walls walls)
    {
        // all the vertices needed for the different faces
        // vertices can't be shared between the triangles, because the shading will be wrong then
        Vector3[] vertices = {
                        new Vector3 (1, 0.5f, 0),
                        new Vector3 (0, 0.5f, 0),
                        new Vector3 (0, 0.5f, 1),
                        new Vector3 (1, 0.5f, 1),

                        new Vector3 (1, 0.5f, 1),
                        new Vector3 (0, 0.5f, 1),
                        new Vector3 (0, -0.5f, 1),
                        new Vector3 (1, -0.5f, 1),

                        new Vector3 (1, -0.5f, 0),
                        new Vector3 (1, 0.5f, 0),
                        new Vector3 (1, 0.5f, 1),
                        new Vector3 (1, -0.5f, 1),

                        new Vector3 (0, -0.5f, 0),
                        new Vector3 (1, 0.5f, 0),
                        new Vector3 (1, -0.5f, 0),
                        new Vector3 (0, 0.5f, 0),

                        new Vector3 (0, -0.5f, 0),
                        new Vector3 (0, -0.5f, 1),
                        new Vector3 (0, 0.5f, 1),
                        new Vector3 (0, 0.5f, 0)
                    };

        int[] faceTop = { 0, 1, 2, 0, 2, 3 };
        int[] faceUp = { 4, 5, 6, 4, 6, 7 };
        int[] faceRight = { 8, 9, 10, 8, 10, 11 };
        int[] faceDown = { 12, 13, 14, 12, 15, 13 };
        int[] faceLeft = { 16, 17, 18, 16, 18, 19 };

        List<int> triangles = new List<int>();

        addMultipleInts(triangles, faceTop);

        if (walls.HasFlag(Walls.Up))
        {
            addMultipleInts(triangles, faceUp);
        }
        if (walls.HasFlag(Walls.Right))
        {
            addMultipleInts(triangles, faceRight);
        }
        if (walls.HasFlag(Walls.Down))
        {
            addMultipleInts(triangles, faceDown);
        }
        if (walls.HasFlag(Walls.Left))
        {
            addMultipleInts(triangles, faceLeft);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void addMultipleInts(List<int> list, int[] ints)
    {
        for (int i = 0; i < ints.Length; i++)
        {
            list.Add(ints[i]);
        }
    }

    private void CombineAllMeshes()
    {
        // initialize
        int vertexLimit = 30000;
        int verticesSoFar = 0;
        MeshFilter[] meshFilters = m_map.GetComponentsInChildren<MeshFilter>();
        List<CombineInstance> combiners = new List<CombineInstance>();
        CombineInstance combine = new CombineInstance();

        // for all the objects within the map
        for (int i = 0; i < meshFilters.Length; i++)
        {
            // if the amount of vertices till now would be over the limit
            if (verticesSoFar + meshFilters[i].mesh.vertexCount > vertexLimit)
            {
                // send list to be combined
                CreateCombinedMeshes(combiners);
                combiners.Clear();
                verticesSoFar = 0;
            }

            // add to the list
            combine.subMeshIndex = 0;
            combine.mesh = meshFilters[i].sharedMesh;
            combine.transform = meshFilters[i].transform.localToWorldMatrix;
            combiners.Add(combine);
            verticesSoFar += combine.mesh.triangles.Length;
        }

        // call the mesh combiner one last time for the last few objects in the list
        CreateCombinedMeshes(combiners);

        // destroy all the objects the map was made up of
        foreach (Transform child in m_map.transform)
        {
            Destroy(child.gameObject);
        }

        // add all the map parts to the map
        foreach (GameObject mapPart in m_mapParts)
        {
            mapPart.transform.SetParent(m_map.transform);
        }
    }

    private void CreateCombinedMeshes(List<CombineInstance> meshDataList)
    {
        // create the combined mesh
        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(meshDataList.ToArray());

        // create new map object to hold part of the map
        GameObject mapPart = Instantiate(Map, Vector3.zero, Quaternion.identity) as GameObject;

        // handle new map object
        mapPart.transform.GetComponent<MeshFilter>().sharedMesh = newMesh;
        var collider = mapPart.GetComponent<MeshCollider>();
        collider.sharedMesh = newMesh;

        // add part to list
        m_mapParts.Add(mapPart);
        mapPart.name = "MapPart " + m_mapParts.Count;
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

    private void DebugMap(int[][] map)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                builder.Append(map[x][y]);
            }
            builder.Append('\n');
        }
        string s = builder.ToString();
        Debug.Log(s);
    }

    private void DebugAddingRoom(int[][] map, int[][] roommap, Vector2 pos)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('\n');
        for (int y = map[0].Length - 1; y >= 0; y--)
        {
            for (int x = 0; x < map.Length; x++)
            {
                if (x >= pos.x && x < pos.x + roommap.Length && y >= pos.y && y < pos.y + roommap[0].Length)
                {
                    builder.Append(roommap[x - (int)pos.x][y - (int)pos.y] + 2);
                }
                else
                {
                    builder.Append(map[x][y]);
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
    public int[][] Roommap;
    public Vector2 Pos;

    public Room(int[][] roomMap)
    {
        Roommap = roomMap;
    }
}

// Walls is used for loading the map
[Flags]
public enum Walls
{
    // Decimal     // Binary
    None = 0,    // 000000
    Up = 1,    // 000001
    Right = 2,    // 000010
    Down = 4,    // 000100
    Left = 8    // 001000
}