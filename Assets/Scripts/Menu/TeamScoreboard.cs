using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class TeamScoreboard : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_teamScorePrefab;
    [SerializeField]
    private GameObject m_teamList;
    [SerializeField]
    private TeamManager m_teamManager;
    private TextMeshProUGUI m_teamScore;

    private void Start()
    {
        if (isServer)
        {
            m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();

            foreach (Team team in m_teamManager.Teams)
            {
                RpcAddTeam(team);
                team.OnScoreChange += RpcRefresh;
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    [ClientRpc]
    private void RpcRefresh(Team team)
    {
        // Get the teamscore
        Transform t = m_teamList.transform.GetChild(team.Id-1); //-1 to match the array
        // Get the TextMesh object
        TextMeshProUGUI textMesh = t.gameObject.GetComponent<TextMeshProUGUI>();
        // Update score
        textMesh.text = team.GetScore().ToString();
        // Set color
        textMesh.outlineColor = team.Color;
        Material mat = Material.Instantiate(textMesh.fontSharedMaterial);
        Color32 teamColor = team.Color;
        // Set alpha because that's not saved in team.Color
        teamColor.a = 255;
        mat.SetColor(ShaderUtilities.ID_GlowColor, teamColor);
        textMesh.fontSharedMaterial = mat;
    }

    [ClientRpc]
    private void RpcAddTeam(Team team)
    {
        // Make a new entry on the scoreboard
        GameObject scorePanel = Instantiate(m_teamScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_teamList.transform, false);

        // Sets the score of the team
        RpcRefresh(team);
    }

    [ClientRpc]
    private void RpcEmpty()
    {
        // Clean the list
        foreach (Transform child in m_teamList.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
