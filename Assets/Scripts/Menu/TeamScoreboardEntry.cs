using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamScoreboardEntry : MonoBehaviour
{
    private TextMeshProUGUI m_kills;

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
        m_kills.text = m_score.Kills.ToString();
        m_deaths.text = m_score.Deaths.ToString();
    }
}
