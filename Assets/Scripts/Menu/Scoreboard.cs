using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        m_teamManager = GameObject.Find("GameManager").GetComponent<TeamManager>();
        if (isServer)
        {
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
        GameObject scorePanel = Instantiate(m_playerScorePrefab) as GameObject;
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
        if(playerScript.Team.Id == 0)
        {
            Debug.Log("Send request to host for updating team-id");
            Team toJoin = m_teamManager.Teams.Aggregate((min, next) => min.PlayerCount < next.PlayerCount ? min : next);
            toJoin.PlayerCount++;
            playerScript.Team = toJoin;
            Debug.Log("Request has been sent and the id should have been updated");
            Debug.Log($"Newest player their team is now {playerScript.Team.Id}");
        }
        playerScript.Team.Score = m_teamManager.Teams[playerScript.Team.Id-1].Score;
        return true;
    }
    [Command]
    private void CmdGetTeamIdFromHost()
    {
        Debug.Log("Host received a message and will now update the team-id of the newest player");
        Soldier playerScript = m_teamManager.GetNewestPlayer();
        RpcUpdateTeamIdForClients(playerScript.Team.Id);
        Debug.Log("Host has updated the team id of the newest player");
    }
    [ClientRpc]
    private void RpcUpdateTeamIdForClients(int teamId)
    {
        Debug.Log("Updating the team Id for a the newest player");
        Soldier playerScript = m_teamManager.GetNewestPlayer();
        playerScript.Team.Id = teamId;
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
