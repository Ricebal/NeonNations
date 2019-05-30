﻿using System.Collections.Generic;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_playerScorePrefab;
    [SerializeField]
    private GameObject m_playerList;
    [SerializeField]
    private GameObject m_scoreBoard;
    [SerializeField]
    private TeamManager m_teamManager;
    private Dictionary<string, ScoreboardEntry> m_outdatedPlayers = new Dictionary<string, ScoreboardEntry>();
    private float m_delayBeforeShowingScoreboard = 3; // The time it waits for showing the scoreboard after the game has finished.

    private void Start()
    {
        GameObject gameManagerObject = GameObject.Find("GameManager");
        m_teamManager = gameManagerObject.GetComponent<TeamManager>();
        if (isServer)
        {
            m_teamManager.OnPlayersChange += Refresh;
            Refresh();
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (!GameManager.GameFinished)
        {
            if (Input.GetKeyDown("tab"))
            {
                SetActive();
            }
            else if (Input.GetKeyUp("tab"))
            {
                m_scoreBoard.SetActive(false);
            }
        }
        else
        {
            m_delayBeforeShowingScoreboard -= Time.deltaTime;
            if(m_delayBeforeShowingScoreboard <=0) // After 3 seconds.
            {
                SetActive(); // Show final scoreboard.
            }
        }

        // If there are outdated players try to add them.
        if (m_outdatedPlayers != null && m_outdatedPlayers.Count > 0)
        {
            RetryOutdatedPlayers();
        }

    }

    private void SetActive()
    {
        m_scoreBoard.SetActive(true);
    }

    private void Refresh()
    {
        if (GameManager.GameFinished)
        {
            return; // prevent players from disappearing on the host score-board once the game has ended.
        }
        // Clear the list and add all players
        RpcEmpty();
        foreach (Soldier player in m_teamManager.GetAllPlayers())
        {
            if (player != null)
            {
                RpcAddPlayer(player.transform.name);
            }
        }
    }

    [ClientRpc]
    private void RpcEmpty()
    {
        // Clean the list
        foreach (Transform child in m_playerList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    private void RpcSortPlayerList()
    {
        int amountOfScorePanels = m_playerList.transform.GetChildCount();
        List<Transform> scorePanels = new List<Transform>();
        for(int i = 0; i < amountOfScorePanels; i++)
        {
            scorePanels.Add(m_playerList.transform.GetChild(i));
        }
        scorePanels = scorePanels.OrderByDescending(entry => entry.gameObject.GetComponent<ScoreboardEntry>().Score.Kills)  // Sort by kills,
                  .ThenBy(entry => entry.gameObject.GetComponent<ScoreboardEntry>().Score.Deaths)                           // Then by lowest deaths.
                  .ToList();
        VerticalLayoutGroup layoutGroup = m_playerList.GetComponent<VerticalLayoutGroup>();
        for (int i = 0; i < amountOfScorePanels; i++)
        {
            // Add the panels to the right place.
            scorePanels[i].SetSiblingIndex(i);
        }
    }

    [ClientRpc]
    private void RpcAddPlayer(string playerId)
    {
        // Make a new entry on the scoreboard
        GameObject scorePanel = Instantiate(m_playerScorePrefab) as GameObject;
        scorePanel.transform.SetParent(m_playerList.transform, false);
        ScoreboardEntry scoreboardEntry = scorePanel.GetComponent<ScoreboardEntry>();
        // Try to add the player to the scoreboard, if it fails add it to the list of outdated players
        if (!AddPlayer(playerId, scoreboardEntry))
        {
            m_outdatedPlayers.Add(playerId, scoreboardEntry);
        }
    }

    private bool AddPlayer(string playerId, ScoreboardEntry scoreboardEntry)
    {
        // Find the player, return false if nothing found
        GameObject player = GameObject.Find(playerId);
        if (player == null)
        {
            return false;
        }

        Soldier playerScript = player.GetComponent<Soldier>();

        Color scoreboardColor = playerScript.InitialColor;
        scoreboardColor.a = 100f/255f; // Set alpha to 100. Color.a works with a value between 0 and 1.
        scoreboardEntry.SetColor(scoreboardColor);
        scoreboardEntry.SetScore(playerScript.PlayerScore);
        scoreboardEntry.Score.OnScoreChange += RpcSortPlayerList;
        if (playerScript.isLocalPlayer)
        {
            scoreboardEntry.EnableOutline();
        }
        return true;
    }

    private void RetryOutdatedPlayers()
    {
        // Make a list of updated players
        List<string> updatedPlayers = new List<string>();
        foreach (KeyValuePair<string, ScoreboardEntry> player in m_outdatedPlayers)
        {
            if (AddPlayer(player.Key, player.Value))
            {
                updatedPlayers.Add(player.Key);
            }
        }
        // Remove updated players from the outdated players list
        updatedPlayers.ForEach(player => m_outdatedPlayers.Remove(player));
    }
}
