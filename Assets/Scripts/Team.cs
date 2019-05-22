using UnityEngine;

[System.Serializable]
public class Team
{
    public int Id;
    public Color Color;
    public int PlayerCount;
    private Score m_score;
    public delegate void OnScoreChangeDelegate(Team team);
    public event OnScoreChangeDelegate OnScoreChange;

    public Team(int id)
    {
        Id = id;
    }

    public void AddKill()
    {
        m_score.Kills++;
        OnScoreChange(this);
    }

    public void AddDeath()
    {
        m_score.Deaths++;
        OnScoreChange(this);
    }
    
    public int GetScore()
    {
        return m_score.Kills;
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
