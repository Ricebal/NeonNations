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

    public Team(int id): this()
    {
        Id = id;
    }
    public Team()
    {
        m_score = new Score();
    }

    public void AddKill()
    {
        m_score.Kills++;
        if (OnScoreChange != null)
        {
            OnScoreChange(this);
        }
    }

    public void AddDeath()
    {
        m_score.Deaths++;
        if (OnScoreChange != null)
        {
            OnScoreChange(this);
        }
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
