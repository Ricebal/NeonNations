using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

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

    private void Start()
    {
        if (isServer)
        {
            m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();
            m_teamManager.OnPlayersChange += Refresh;
            Refresh();
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown("tab"))
        {
            m_scoreBoard.SetActive(true);
        }
        else if (Input.GetKeyUp("tab"))
        {
            m_scoreBoard.SetActive(false);
        }

        // If there are outdated players try to add them.
        if (m_outdatedPlayers != null && m_outdatedPlayers.Count > 0)
        {
            RetryOutdatedPlayers();
        }
    }

    private void Refresh()
    {
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
    private void RpcAddPlayer(string playerId)
    {
        // Make a new entry on the scoreboard
        GameObject scorePanel = Instantiate(m_playerScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_playerList.transform, false);

        // Try to add the player to the scoreboard, if it fails add it to the list of outdated players
        if (!AddPlayer(playerId, scorePanel.GetComponent<ScoreboardEntry>()))
        {
            m_outdatedPlayers.Add(playerId, scorePanel.GetComponent<ScoreboardEntry>());
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

        scoreboardEntry.SetScore(playerScript.PlayerScore);
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
