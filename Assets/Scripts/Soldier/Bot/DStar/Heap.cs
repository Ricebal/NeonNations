using System;
using System.Collections.Generic;

public class Heap // minheap
{
    private int m_count;
    private HeapElement[] m_heap;
    private Dictionary<Node, int> m_hash;

    public Heap(int cap)
    {
        m_count = 0;
        m_heap = new HeapElement[cap];
        m_hash = new Dictionary<Node, int>();
    }

    public override string ToString()
    {
        string s = "";
        foreach(Node n in m_hash.Keys)
        {
            s += $"{n.X}, {n.Y} \n";
        }
        return s;
    }

    public PriorityKey TopKey()
    {
        if (m_count == 0) return new PriorityKey(double.PositiveInfinity, double.PositiveInfinity);
        return m_heap[1].Key;
    }

    public Node Pop()
    {
        if (m_count == 0) return null;
        Node s = m_heap[1].Node;
        m_heap[1] = m_heap[m_count];
        m_hash[m_heap[1].Node] = 1;
        m_hash[s] = 0;
        m_count--;
        moveDown(1);
        return s;
    }

    public void Insert(Node s, PriorityKey k)
    {
        HeapElement e = new HeapElement(s, k);
        m_count++;
        m_hash[s] = m_count;
        if (m_count == m_heap.Length)
        {
            increaseCap();
        }
        m_heap[m_count] = e;
        moveUp(m_count);
    }

    public void Remove(Node s)
    {
        int i = m_hash[s];
        if (i == 0) return;
        m_hash[s] = 0;
        m_heap[i] = m_heap[m_count];
        m_hash[m_heap[i].Node] = i;
        m_count--;
        moveDown(i);
    }

    public bool Contains(Node s)
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
        m_hash[m_heap[j].Node] = i;
        m_heap[j] = temp;
        m_hash[temp.Node] = j;
    }

    private void increaseCap()
    {
        Array.Resize<HeapElement>(ref m_heap, m_heap.Length * 2);
    }
}