using UnityEngine;

[System.Serializable]
public class Team
{
    public int Id;
    public Color Color;
    public int PlayerCount;

    public Team(int id)
    {
        Id = id;
    }
}
