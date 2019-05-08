public class PriorityKey
{
    public double Key1;
    public double Key2;

    public PriorityKey(double k1, double k2)
    {
        this.Key1 = k1;
        this.Key2 = k2;
    }

    public int CompareTo(PriorityKey that)
    {
        if (this.Key1 < that.Key1) return -1;
        else if (this.Key1 > that.Key1) return 1;
        if (this.Key2 > that.Key2) return 1;
        else if (this.Key2 < that.Key2) return -1;
        return 0;
    }
}
