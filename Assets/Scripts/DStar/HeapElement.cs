using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HeapElement
{
    public Vector2Int Coordinates;
    public PriorityKey Key;

    public HeapElement(Vector2Int coordinates, PriorityKey key)
    {
        Coordinates = coordinates;
        Key = key;
    }
}
