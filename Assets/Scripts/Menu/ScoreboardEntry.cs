using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_username;
    [SerializeField]
    private TextMeshProUGUI m_kills;
    [SerializeField]
    private TextMeshProUGUI m_deaths;
    [SerializeField]
    private Outline m_outline;
    private Score m_score;

    public void SetScore(Score score)
    {
        m_score = score;
        score.OnScoreChange += Refresh;
        Refresh();
    }

    public void EnableOutline()
    {
        m_outline.enabled = true;
    }

    private void Refresh()
    {
        m_username.text = m_score.Username;
        m_kills.text = m_score.Kills.ToString();
        m_deaths.text = m_score.Deaths.ToString();
    }
}
