using UnityEngine;

[System.Serializable]
public class Score
{
    [SerializeField]
    private int m_kills = 0;
    [SerializeField]
    private int m_deaths = 0;
    public delegate void OnScoreChangeDelegate();
    public event OnScoreChangeDelegate OnScoreChange;
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
    public int GetScore()
    {
        return Kills; // Currently just for team death match.
    }

    /// <summary>
    /// Returns the score, depending on the gamemode. Ready to be used by a GUI.
    /// </summary>
    public string GetScoreForGUI()
    {
        return GetScoreForGUI().ToString(); // Currently just for team death match.
    }
}
