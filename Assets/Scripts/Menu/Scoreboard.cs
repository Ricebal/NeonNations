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
    private TeamManager m_teamManager;

    private void Start()
    {
        m_teamManager.OnPlayersChange += CmdRefresh;
        CmdRefresh();
    }

    [Command]
    public void CmdRefresh()
    {
        RpcEmpty();
    }

    [ClientRpc]
    private void RpcEmpty()
    {
        foreach (Transform child in m_playerList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Soldier player in m_teamManager.GetAllPlayers())
        {
            if (player != null)
            {
                Debug.Log("Soldiers!");
                GameObject scorePanel = Instantiate(m_playerScorePrefab)as GameObject;
                scorePanel.transform.SetParent(m_playerList.transform, false);
            }
        }
    }
}
