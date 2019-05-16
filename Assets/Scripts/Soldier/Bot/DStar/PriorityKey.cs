/// <summary>
/// Entries are ordered by k1 (which corresponds directly to the f-values used in A*), then by k2.
/// </summary>
namespace Assets.Scripts.Soldier.Bot.DStar
{
    public class PriorityKey
    {
        public double Key1;
        public double Key2;

        public PriorityKey(double k1, double k2)
        {
            Key1 = k1;
            Key2 = k2;
        }

        public int CompareTo(PriorityKey that)
        {
            if (Key1 < that.Key1) return -1;
            else if (Key1 > that.Key1) return 1;
            if (Key2 > that.Key2) return 1;
            else if (Key2 < that.Key2) return -1;
            return 0;
        }
    }
}