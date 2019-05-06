using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K
{
    public double k1;
    public double k2;

    public K(double K1, double K2)
    {
        k1 = K1;
        k2 = K2;
    }

    public int CompareTo(K that)
    {
        if (this.k1 < that.k1) return -1;
        else if (this.k1 > that.k1) return 1;
        if (this.k2 > that.k2) return 1;
        else if (this.k2 < that.k2) return -1;
        return 0;
    }
}
