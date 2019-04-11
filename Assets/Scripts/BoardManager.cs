using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 30;
    public int rows = 22;
    public int outerWallWidth = 2;

    private bool[,] tileMap;
    private Transform boardHolder;

    void GenerateMap()
    {
        tileMap = new bool[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (// rooms
                    x == 0 && y == 0
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
                    || x >= 25 && x <= 26 && y >= 11 && y <= 16)
                {
                    tileMap[x, y] = true;
                }
            }
        }
    }

    void LoadMap()
    {
        boardHolder = new GameObject("Board").transform;
        for (int i = 0; i < tileMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMap.GetLength(1); j++)
            {
                if (!tileMap[i, j]) // if false, build wall;
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(boardHolder);
                }
            }
        }
    }

    void GenerateOuterWall()
    {
        for (int i = -outerWallWidth; i < tileMap.GetLength(0) + outerWallWidth; i++)
        {
            for (int j = -outerWallWidth; j < tileMap.GetLength(1) + outerWallWidth; j++)
            {
                if (i < 0 && j != 0 || i >= tileMap.GetLength(0) || j < 0 || j >= tileMap.GetLength(1))
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(i, 0f, j);
                    instance.transform.SetParent(boardHolder);
                }
            }
        }
    }

    void GenerateFloor()
    {
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.transform.position = new Vector3((float)columns/2 - 0.5f, -0.5f, (float)rows / 2 - 0.5f);
        floor.transform.localScale = new Vector3((float)(columns + outerWallWidth*2) / 10, 1, (float)(rows + outerWallWidth*2) / 10);
        floor.transform.SetParent(boardHolder);
    }

    public void SetupScene()
    {
        GenerateMap();
        GenerateFloor();
        LoadMap();
        GenerateOuterWall();
    }
}
