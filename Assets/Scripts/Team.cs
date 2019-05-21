using UnityEngine;

[System.Serializable]
public class Team
{
    public int Id;
    public Color Color;
    public int PlayerCount;
    public Score Score;

    public Team(int id)
    {
        Id = id;
    }

    public override bool Equals(object obj)
    {
        if(obj is Team casted)
        {
            return casted.Id == Id;
        }

        return false;
    }
}
