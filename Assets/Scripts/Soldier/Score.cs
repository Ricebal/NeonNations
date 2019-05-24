public class Score
{
    private int m_kills = 0;
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
}
