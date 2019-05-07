﻿using System;
using System.Collections.Generic;

public class Heap //minheap
{
    private int m_count;
    private HeapElement[] m_heap;
    private Dictionary<State, int> m_hash;

    public Heap(int cap)
    {
        m_count = 0;
        m_heap = new HeapElement[cap];
        m_hash = new Dictionary<State, int>();
    }

    public Key TopKey()
    {
        if (m_count == 0) return new Key(double.PositiveInfinity, double.PositiveInfinity);
        return m_heap[1].Key;
    }

    public State Pop()
    {
        if (m_count == 0) return null;
        State s = m_heap[1].State;
        m_heap[1] = m_heap[m_count];
        m_hash[m_heap[1].State] = 1;
        m_hash[s] = 0;
        m_count--;
        moveDown(1);
        return s;
    }

    public void Insert(State s, Key k)
    {
        HeapElement e = new HeapElement(s, k);
        m_count++;
        m_hash[s] = m_count;
        if (m_count == m_heap.Length) increaseCap();
        m_heap[m_count] = e;
        moveUp(m_count);
    }

    public void Update(State s, Key k)
    {
        int i = m_hash[s];
        if (i == 0) return;
        Key kold = m_heap[i].Key;
        m_heap[i].Key = k;
        if (kold.CompareTo(k) < 0)
        {
            moveDown(i);
        }
        else
        {
            moveUp(i);
        }
    }

    public void Remove(State s)
    {
        int i = m_hash[s];
        if (i == 0) return;
        m_hash[s] = 0;
        m_heap[i] = m_heap[m_count];
        m_hash[m_heap[i].State] = i;
        m_count--;
        moveDown(i);
    }

    public bool Contains(State s)
    {
        int i;
        if (!m_hash.TryGetValue(s, out i))
        {
            return false;
        }
        return i != 0;
    }

    private void moveDown(int i)
    {
        int childL = i * 2;
        if (childL > m_count) return;
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
            swap(i, smallerChild);
            moveDown(smallerChild);
        }
    }

    private void moveUp(int i)
    {
        if (i == 1) return;
        int parent = i / 2;
        if (m_heap[parent].Key.CompareTo(m_heap[i].Key) > 0)
        {
            swap(parent, i);
            moveUp(parent);
        }
    }

    private void swap(int i, int j)
    {
        HeapElement temp = m_heap[i];
        m_heap[i] = m_heap[j];
        m_hash[m_heap[j].State] = i;
        m_heap[j] = temp;
        m_hash[temp.State] = j;
    }

    private void increaseCap()
    {
        Array.Resize<HeapElement>(ref m_heap, m_heap.Length * 2);
    }
}