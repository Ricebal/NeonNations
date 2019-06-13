using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
    [SerializeField] private Image m_scoreboardEntryBackground = null;
    [SerializeField] private TextMeshProUGUI m_username = null;
    [SerializeField] private TextMeshProUGUI m_kills = null;
    [SerializeField] private TextMeshProUGUI m_deaths = null;
    [SerializeField] private Outline m_outline = null;
    public Score Score;

    public void SetScore(Score score)
    {
        Score = score;
        score.OnScoreChange += Refresh;
        Refresh();
    }

    public void SetColor(Color color)
    {
        m_scoreboardEntryBackground.color = new Color(color.r, color.g, color.b, m_scoreboardEntryBackground.color.a);
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
