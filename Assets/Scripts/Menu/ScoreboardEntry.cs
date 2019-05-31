using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
    [SerializeField]
    private Image m_scoreboardEntryBackground;
    [SerializeField]
    private TextMeshProUGUI m_username;
    [SerializeField]
    private TextMeshProUGUI m_kills;
    [SerializeField]
    private TextMeshProUGUI m_deaths;
    [SerializeField]
    private Outline m_outline;
    public Score Score;

    public void SetScore(Score score)
    {
        Score = score;
        score.OnScoreChange += Refresh;
        Refresh();
    }

    public void SetColor(Color color)
    {
        m_scoreboardEntryBackground.color = color;
    }

    public void EnableOutline()
    {
        m_outline.enabled = true;
    }

    private void Refresh()
    {
        m_username.text = Score.Username;
        m_kills.text = Score.Kills.ToString();
        m_deaths.text = Score.Deaths.ToString();
    }
}
