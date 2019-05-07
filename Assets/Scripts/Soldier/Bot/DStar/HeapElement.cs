using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapElement
{
    public State State;
    public Key Key;

    public HeapElement(State state, Key key)
    {
        State = state;
        Key = key;
    }
}
