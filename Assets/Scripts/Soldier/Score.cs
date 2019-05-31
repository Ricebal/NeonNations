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
            OnScoreChange?.Invoke();
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
            OnScoreChange?.Invoke();
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
            OnScoreChange?.Invoke();
        }
    }
}
