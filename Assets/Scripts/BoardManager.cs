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

    private Transform boardHolder;

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x <= columns; x++)
        {
            for (int z = -1; z <= rows; z++)
            {
                if (x == -1 && z !=0 
                    || x == columns 
                    || z == -1 
                    || z == rows 
                    || x == 0 && z >= 1 && z <= 15 
                    || x >= 1 && x <= 4 && z >= 6 && z <= 11 
                    || x >= 3 && x <= 6 && z >= 14 && z <= 15
                    || x >= 7 && x <= 15 && z >= 11 && z <= 15
                    || x >= 10 && x <= 15 && z == 16
                    || x >= 7 && x <= 10 && z == 10
                    || x == 16 && z >= 10 && z <= 12
                    || x >= 10 && x <= 16 && z >= 19 && z <= 21
                    || x >= 7 && x <= 10 && z >= 6 && z <= 7
                    || x >= 8 && x <= 10 && z >= 4 && z <= 5
                    || x >= 8 && x <= 12 && z >= 2 && z <= 3
                    || x >= 15 && x <= 22 && z >= 2 && z <= 3
                    || x >= 16 && x <= 22 && z >= 4 && z <= 7
                    || x >= 25 && x <= 29 && z >= 0 && z <= 2
                    || x >= 28 && x <= 29 && z >= 11 && z <= 21
                    || x == 27 && z >= 11 && z <= 16
                    || x >= 19 && x <= 23 && z == 19
                    || x >= 21 && x <= 23 && z >= 11 && z <= 18
                    || x == 24 && z >= 11 && z <= 16
                    || x >= 19 && x <= 20 && z >= 11 && z <= 13
                    || x >= 19 && x <= 22 && z == 10)
                {
                    GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    instance.transform.position = new Vector3(x, 0f, z);
                    instance.transform.SetParent(boardHolder);
                }
            }
        }
    }

    void LayoutObject()
    {

    }

    public void SetupScene()
    {
        BoardSetup();
    }
}
