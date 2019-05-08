using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapElement
{
    public State State;
    public PriorityKey Key;

    public HeapElement(State state, PriorityKey key)
    {
        State = state;
        Key = key;
    }
}
