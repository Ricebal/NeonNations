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
            foreach(Team team in m_teamManager.Teams)
            {
                RpcAddTeam(team);
                team.OnScoreChange += Refresh;
                Refresh(team);
            }
        }

        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    private void Refresh(Team team)
    {
        // Get the teamscore
        Transform t = m_teamList.transform.GetChild(team.Id);
        // Update score
        t.gameObject.GetComponent<TextMeshProUGUI>().text = team.GetScore().ToString();
    }

    [ClientRpc]
    private void RpcAddTeam(Team team)
    {
        // Make a new entry on the scoreboard
        GameObject scorePanel = Instantiate(m_teamScorePrefab)as GameObject;
        scorePanel.transform.SetParent(m_teamList.transform, false);

        // Sets the score of the team
        scorePanel.GetComponent<TextMeshProUGUI>().text = team.GetScore().ToString();
    }

    //private bool AddTeam(Team team, ScoreboardEntry scoreboardEntry)
    //{
    //    // Find the player, return false if nothing found
    //    if (player == null)
    //    {
    //        return false;
    //    }

    //    Soldier playerScript = player.GetComponent<Soldier>();

    //    scoreboardEntry.SetScore(playerScript.PlayerScore);
    //    if (playerScript.isLocalPlayer)
    //    {
    //        scoreboardEntry.EnableOutline();
    //    }
    //    return true;
    //}

    //private void RetryOutdatedPlayers()
    //{
    //    // Make a list of updated players
    //    List<string> updatedPlayers = new List<string>();
    //    foreach (KeyValuePair<string, ScoreboardEntry> player in m_outdatedPlayers)
    //    {
    //        if (AddTeam(player.Key, player.Value))
    //        {
    //            updatedPlayers.Add(player.Key);
    //        }
    //    }
    //    // Remove updated players from the outdated players list
    //    updatedPlayers.ForEach(player => m_outdatedPlayers.Remove(player));
    //}
}
