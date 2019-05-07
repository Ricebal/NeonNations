public class Key
{
    public double Key1;
    public double Key2;

    public Key(double k1, double k2)
    {
        this.Key1 = k1;
        this.Key2 = k2;
    }

    public int CompareTo(Key that)
    {
        if (this.Key1 < that.Key1) return -1;
        else if (this.Key1 > that.Key1) return 1;
        if (this.Key2 > that.Key2) return 1;
        else if (this.Key2 < that.Key2) return -1;
        return 0;
    }
}
