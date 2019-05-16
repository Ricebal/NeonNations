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
                RpcAddPlayer("PlayerName", 10, 5);
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
    private void RpcAddPlayer(string name, int kills, int deaths)
    {
        Debug.Log("Adding soldier with name: " + name + ", kills: " + kills + ", deaths: " + deaths);
        GameObject scorePanel = Instantiate(m_playerScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_playerList.transform, false);
    }
}
