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
        Coordinates that = (Coordinates)obj;
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }

    public bool Equals(Coordinates that)
    {
        if (this.X == that.X && this.Y == that.Y) return true;
        return false;
    }
}