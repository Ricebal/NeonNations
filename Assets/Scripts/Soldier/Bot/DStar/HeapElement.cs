using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapElement
{
    public State s;
    public K k;

    public HeapElement(State state, K key)
    {
        s = state;
        k = key;
    }
}
