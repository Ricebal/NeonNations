using UnityEngine;

public class Score
{
    private string m_username = "Unknown";
    private int m_kills = 0;
    private int m_deaths = 0;

    public delegate void OnScoreChangeDelegate();
    public event OnScoreChangeDelegate OnScoreChange;

    public string Username
    {
        get
        {
            return m_username;
        }
        set
        {
            m_username = value;
            if (OnScoreChange != null)
            {
                OnScoreChange();
            }
        }
    }

    public int Kills
    {
        get
        {
            return m_kills;
        }
        set
        {
            m_kills = value;
            if (OnScoreChange != null)
            {
                OnScoreChange();
            }
        }
    }

    public int Deaths
    {
        get
        {
            return m_deaths;
        }
        set
        {
            m_deaths = value;
            if (OnScoreChange != null)
            {
                OnScoreChange();
            }
        }
    }

    public Score() : this(0, 0) { }

    public Score(int kills, int deaths)
    {
        m_kills = kills;
        m_deaths = deaths;
    }
    
    /// <summary>
    /// Returns the score, depending on the gamemode.
    /// </summary>
    public int GetScore(GameModes gameMode)
    {
        switch (gameMode)
        {
            case (GameModes.TeamDeathMatch):
                return Kills;
        }
        return 0;
    }

    /// <summary>
    /// Returns the score, depending on the gamemode. Ready to be used by a GUI.
    /// </summary>
    public string GetScoreAsString(GameModes gameMode)
    {
        return GetScore(gameMode).ToString();
    }
}
