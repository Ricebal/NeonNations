public class Coordinates
{
    public int X;
    public int Y;

    public Coordinates()
    {
        this.X = 0;
        this.Y = 0;
    }
    public Coordinates(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override bool Equals(object obj)
    {
        if(obj == null)
        {
            return false;
        }
        Coordinates that = obj as Coordinates;
        if (that == null)
        {
            return false;
        }
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }

    public bool Equals(Coordinates that)
    {
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }
    public override int GetHashCode()
    {
        return GetHashCodeInternal(X.GetHashCode(), Y.GetHashCode());
    }
    //this function should be move so you can reuse it
    private static int GetHashCodeInternal(int key1, int key2)
    {
        unchecked
        {
            //Seed
            var num = 0x7e53a269;

            //Key 1
            num = (-1521134295 * num) + key1;
            num += (num << 10);
            num ^= (num >> 6);

            //Key 2
            num = ((-1521134295 * num) + key2);
            num += (num << 10);
            num ^= (num >> 6);

            return num;
        }
    }
}