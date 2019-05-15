using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapElement
{
    public Coordinates Coordinates;
    public PriorityKey Key;

    public HeapElement(Coordinates coordinates, PriorityKey key)
    {
        Coordinates = coordinates;
        Key = key;
    }
}
