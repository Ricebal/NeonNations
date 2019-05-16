using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_name;
    [SerializeField]
    private TextMeshProUGUI m_kills;
    [SerializeField]
    private TextMeshProUGUI m_deaths;
    private Score m_score;

    public void SetScore(Score score)
    {
        m_score = score;
        score.OnScoreChange += Refresh;
    }

    private void Refresh()
    {
        m_kills.text = m_score.Kills.ToString();
        m_deaths.text = m_score.Deaths.ToString();
    }
}
