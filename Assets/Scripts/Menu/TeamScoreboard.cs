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
    private Dictionary<int, TeamScoreboardEntry> m_teamScoreboardEntries;

    private void Start()
    {
        if (isServer)
        {
            m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();
            m_teamManager.OnPlayersChange += RefreshScores;
            m_teamScoreboardEntries = new Dictionary<int, TeamScoreboardEntry>();
            foreach (Team team in m_teamManager.Teams)
            {
                RpcAddTeam(team.Id);
            }
            RpcRefreshScores();
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    [ClientRpc]
    private void RpcAddTeam(int teamId)
    {
        // Get team, -1 for array.
        Team team = m_teamManager.Teams[teamId - 1];

        // Make a new entry on the scoreboard
        GameObject scorePanel = Instantiate(m_teamScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_teamList.transform, false);

        // Set the score
        TeamScoreboardEntry entry = scorePanel.GetComponent<TeamScoreboardEntry>();
        entry.SetScore(team.Score);
        entry.SetColor(team.Color);
        m_teamScoreboardEntries.Add(teamId, entry);
    }

    private void RefreshScores()
    {
        RpcRefreshScores();
    }

    [ClientRpc]
    private void RpcRefreshScores()
    {
        foreach (KeyValuePair<int, TeamScoreboardEntry> keyValuePair in m_teamScoreboardEntries)
        {
            // Get team, -1 for array.
            Team team = m_teamManager.Teams[keyValuePair.Key - 1];
            keyValuePair.Value.SetScore(team.Score);
        }
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
