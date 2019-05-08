using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapElement
{
    public Node Node;
    public PriorityKey Key;

    public HeapElement(Node node, PriorityKey key)
    {
        Node = node;
        Key = key;
    }
}
