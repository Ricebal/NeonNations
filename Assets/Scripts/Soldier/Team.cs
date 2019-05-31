using UnityEngine;

[System.Serializable]
public class Team
{
    public int Id;
    public Color Color;
    public int PlayerCount;
    [SerializeField]
    public Score Score;

    public Team(int id) : this()
    {
        Id = id;
    }
    public Team()
    {
        Score = new Score();
        Id = 0;
    }

    public void AddKill()
    {
        Score.Kills++;
    }

    public void AddDeath()
    {
        Score.Deaths++;
    }

    public override bool Equals(object obj)
    {
        if (obj is Team casted)
        {
            return casted.Id == Id;
        }

        return false;
    }
}
