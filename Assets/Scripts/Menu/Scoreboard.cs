using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    private Dictionary<string, ScoreboardEntry> m_outdatedPlayers;

    private void Start()
    {
        if (isServer)
        {
            m_teamManager.OnPlayersChange += Refresh;
            Refresh();
        }
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
        if (m_outdatedPlayers != null)
        {
            RetryOutdatedPlayers();
        }
    }

    private void Refresh()
    {
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
        Debug.Log("RpcEmpty");
        foreach (Transform child in m_playerList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    private void RpcAddPlayer(string playerId)
    {
        if (m_outdatedPlayers == null)
        {
            m_outdatedPlayers = new Dictionary<string, ScoreboardEntry>();
        }
        Debug.Log("RpcAddPlayer(" + playerId + ")");
        GameObject scorePanel = Instantiate(m_playerScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_playerList.transform, false);

        if (!AddPlayer(playerId, scorePanel.GetComponent<ScoreboardEntry>()))
        {
            m_outdatedPlayers.Add(playerId, scorePanel.GetComponent<ScoreboardEntry>());
        }
    }

    private bool AddPlayer(string playerId, ScoreboardEntry scoreboardEntry)
    {
        GameObject shooter = GameObject.Find(playerId);
        if (shooter == null)
        {
            return false;
        }

        scoreboardEntry.SetScore(shooter.GetComponent<Soldier>().PlayerScore);
        return true;
    }

    private void RetryOutdatedPlayers()
    {
        List<string> updatedPlayers = new List<string>();
        foreach (KeyValuePair<string, ScoreboardEntry> player in m_outdatedPlayers)
        {
            if (AddPlayer(player.Key, player.Value))
            {
                updatedPlayers.Add(player.Key);
            }
        }
        updatedPlayers.ForEach(player => m_outdatedPlayers.Remove(player));
    }
}
