using System;
using System.Collections.Generic;

public class Heap //minheap
{
    private int n;
    private HeapElement[] heap;
    private Dictionary<State, int> hash;

    public Heap(int cap)
    {
        n = 0;
        heap = new HeapElement[cap];
        hash = new Dictionary<State, int>();
    }

    public K TopKey()
    {
        if (n == 0) return new K(double.PositiveInfinity, double.PositiveInfinity);
        return heap[1].k;
    }

    public State Pop()
    {
        if (n == 0) return null;
        State s = heap[1].s;
        heap[1] = heap[n];
        hash[heap[1].s] = 1;
        hash[s] = 0;
        n--;
        moveDown(1);
        return s;
    }

    public void Insert(State s, K k)
    {
        HeapElement e = new HeapElement(s, k);
        n++;
        hash[s] = n;
        if (n == heap.Length) increaseCap();
        heap[n] = e;
        moveUp(n);
    }

    public void Update(State s, K k)
    {
        int i = hash[s];
        if (i == 0) return;
        K kold = heap[i].k;
        heap[i].k = k;
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
        int i = hash[s];
        if (i == 0) return;
        hash[s] = 0;
        heap[i] = heap[n];
        hash[heap[i].s] = i;
        n--;
        moveDown(i);
    }

    public bool Contains(State s)
    {
        int i;
        if (!hash.TryGetValue(s, out i))
        {
            return false;
        }
        return i != 0;
    }

    private void moveDown(int i)
    {
        int childL = i * 2;
        if (childL > n) return;
        int childR = i * 2 + 1;
        int smallerChild;
        if (childR > n)
        {
            smallerChild = childL;
        }
        else if (heap[childL].k.CompareTo(heap[childR].k) < 0)
        {
            smallerChild = childL;
        }
        else
        {
            smallerChild = childR;
        }
        if (heap[i].k.CompareTo(heap[smallerChild].k) > 0)
        {
            swap(i, smallerChild);
            moveDown(smallerChild);
        }
    }

    private void moveUp(int i)
    {
        if (i == 1) return;
        int parent = i / 2;
        if (heap[parent].k.CompareTo(heap[i].k) > 0)
        {
            swap(parent, i);
            moveUp(parent);
        }
    }

    private void swap(int i, int j)
    {
        HeapElement temp = heap[i];
        heap[i] = heap[j];
        hash[heap[j].s] = i;
        heap[j] = temp;
        hash[temp.s] = j;
    }

    private void increaseCap()
    {
        Array.Resize<HeapElement>(ref heap, heap.Length * 2);
    }
}