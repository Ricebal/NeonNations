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

    private void Start()
    {
        if (isServer)
        {
            m_teamManager.OnPlayersChange += Refresh;
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
    }

    public void Refresh()
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
        foreach (Transform child in m_playerList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    [ClientRpc]
    private void RpcAddPlayer(string shooterId)
    {
        Debug.Log("Adding player: " + shooterId);
        GameObject scorePanel = Instantiate(m_playerScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_playerList.transform, false);
        GameObject shooter = GameObject.Find(shooterId);
        if (shooter != null)
        {
            scorePanel.GetComponent<ScoreboardEntry>().SetScore(shooter.GetComponent<Soldier>().PlayerScore);
        }
    }
}
