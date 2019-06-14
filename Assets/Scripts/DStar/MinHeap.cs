/**
 * Authors: Benji
 */

using System;
using System.Collections.Generic;
using UnityEngine;

public class MinHeap
{
    private int m_count;
    private HeapElement[] m_heap;
    private Dictionary<Vector2Int, int> m_hash;

    public MinHeap(int cap)
    {
        m_count = 0;
        m_heap = new HeapElement[cap];
        m_hash = new Dictionary<Vector2Int, int>();
    }

    /// <summary>
    /// Returns the key on top of the heap
    /// </summary>
    public PriorityKey TopKey()
    {
        if (m_count == 0)
        {
            return new PriorityKey(double.PositiveInfinity, double.PositiveInfinity);
        }
        return m_heap[1].Key;
    }

    /// <summary>
    /// Returns the coordinates on top of the heap and will remove these coorinates from the heap
    /// </summary>
    /// <returns>Vector2Int</returns>
    public Vector2Int Pop()
    {
        if (m_count <= 0)
        {
            m_count = 0;
            return new Vector2Int();
        }
        Vector2Int coordinates = m_heap[1].Coordinates;
        m_heap[1] = m_heap[m_count];
        m_hash[m_heap[1].Coordinates] = 1;
        m_hash[coordinates] = 0;
        m_count--;
        MoveDown(1);
        return coordinates;
    }

    /// <summary>
    /// Inserts an item in the Heap
    /// </summary>
    public void Insert(Vector2Int coordinates, PriorityKey key)
    {
        HeapElement e = new HeapElement(coordinates, key);
        m_count++;
        m_hash[coordinates] = m_count;
        if (m_count == m_heap.Length)
        {
            IncreaseCap();
        }
        m_heap[m_count] = e;
        MoveUp(m_count);
    }

    /// <summary>
    /// Removes a coordinate from the heap
    /// </summary>
    /// <param name="coordinates">The coordinates to be removed</param>
    public void Remove(Vector2Int coordinates)
    {
        int i = m_hash[coordinates];
        if (i == 0)
        {
            return;
        }
        if (m_count <= 0)
        {
            m_count = 0;
            return;
        }
        m_hash[coordinates] = 0;
        m_heap[i] = m_heap[m_count];
        m_hash[m_heap[i].Coordinates] = i;
        m_count--;
        MoveDown(i);
    }

    /// <summary>
    /// Checks if a coordinate is already in the heap
    /// </summary>
    /// <param name="coordinates">The coordinates that need to be checked</param>
    public bool Contains(Vector2Int coordinates)
    {
        int i;
        if (!m_hash.TryGetValue(coordinates, out i))
        {
            return false;
        }
        return i != 0;
    }

    private void MoveDown(int i)
    {
        int childL = i * 2;
        if (childL > m_count)
        {
            return;
        }
        int childR = i * 2 + 1;
        int smallerChild;
        if (childR > m_count)
        {
            smallerChild = childL;
        }
        else if (m_heap[childL].Key.CompareTo(m_heap[childR].Key) < 0)
        {
            smallerChild = childL;
        }
        else
        {
            smallerChild = childR;
        }
        if (m_heap[i].Key.CompareTo(m_heap[smallerChild].Key) > 0)
        {
            Swap(i, smallerChild);
            MoveDown(smallerChild);
        }
    }

    private void MoveUp(int i)
    {
        if (i == 1)
        {
            return;
        }
        int parent = i / 2;
        if (m_heap[parent].Key.CompareTo(m_heap[i].Key) > 0)
        {
            Swap(parent, i);
            MoveUp(parent);
        }
    }

    private void Swap(int i, int j)
    {
        HeapElement temp = m_heap[i];
        m_heap[i] = m_heap[j];
        m_hash[m_heap[j].Coordinates] = i;
        m_heap[j] = temp;
        m_hash[temp.Coordinates] = j;
    }

    private void IncreaseCap()
    {
        Array.Resize<HeapElement>(ref m_heap, m_heap.Length * 2);
    }
}
