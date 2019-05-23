using TMPro;
using UnityEngine;

public class TeamScoreboardEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_teamScore;
    private Score m_score;

    public void SetScore(Score score)
    {
        m_score = score;
        m_score.OnScoreChange += UpdateScore;
        UpdateScore();
    }

    private void UpdateScore()
    {
        m_teamScore.text = m_score.Kills.ToString();
    }

    public void SetColor(Color teamColor)
    {
        // Set color
        m_teamScore.outlineColor = teamColor;
        Material mat = Material.Instantiate(m_teamScore.fontSharedMaterial);
        Color32 teamColor32 = teamColor;
        // Set alpha because that's not saved in team.Color
        teamColor32.a = 255;
        mat.SetColor(ShaderUtilities.ID_GlowColor, teamColor32);
        m_teamScore.fontSharedMaterial = mat;
    }
}
