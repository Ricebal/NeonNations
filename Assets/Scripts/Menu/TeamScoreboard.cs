using Mirror;
using System.Collections.Generic;
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
    private GameMode m_gameMode;
    [SerializeField]
    private TextMeshProUGUI m_remainingTime;
    private Dictionary<int, TeamScoreboardEntry> m_teamScoreboardEntries = new Dictionary<int, TeamScoreboardEntry>();

    private void Start()
    {
        m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_gameMode = gameManager.GameMode;
        foreach (Team team in m_teamManager.Teams)
        {
            AddTeam(team.Id);
            if (isServer)
            {
                team.Score.OnScoreChange += RefreshScores;
            }
            team.Score.OnScoreChange += m_gameMode.CheckForWinCondition;
        }

        if (isServer)
        {
            m_teamManager.OnPlayersChange += RefreshScores;
            RpcRefreshScores();
        }

        GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -37.5f); // -37.5 for the offset. Otherwise the Score-display would be only half on screen.
    }

    private void FixedUpdate()
    {
        m_remainingTime.text = m_gameMode.RemainingTimeAsText();
    }

    private void AddTeam(int teamId)
    {
        // Get team, -1 for array.
        Team team = m_teamManager.Teams[teamId - 1];

        // Make a new entry on the scoreboard
        GameObject scorePanel = Instantiate(m_teamScorePrefab) as GameObject;
        scorePanel.transform.SetParent(m_teamList.transform, false);

        // Set the score
        TeamScoreboardEntry entry = scorePanel.GetComponent<TeamScoreboardEntry>();
        entry.SetGameMode(m_gameMode);
        entry.SetScore(team.Score);
        entry.SetColor(team.Color);
        m_teamScoreboardEntries.Add(teamId, entry);
    }

    private void RefreshScores()
    {
        m_teamManager.SyncTeams();
        RpcRefreshScores();
    }

    [ClientRpc]
    private void RpcRefreshScores()
    {
        foreach (KeyValuePair<int, TeamScoreboardEntry> keyValuePair in m_teamScoreboardEntries)
        {
            Team team = m_teamManager.Teams[keyValuePair.Key - 1];
            keyValuePair.Value.SetScore(team.Score);
        }
    }
}
