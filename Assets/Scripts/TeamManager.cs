using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public List<Team> Teams;
    private int m_playerCount;
    private Soldier[] m_players = new Soldier[8];
    public delegate void OnPlayersChangeDelegate();
    public event OnPlayersChangeDelegate OnPlayersChange;

    public Team AddPlayer(Soldier player)
    {
        Team team = new Team(-1);

        // Add player to list of players
        m_players[m_playerCount] = player;

        // If there's no team throw a debug
        if (Teams.Count > 0)
        {
            // Get the team with the least players
            Team toJoin = Teams.Aggregate((min, next) => min.PlayerCount < next.PlayerCount ? min : next);
            toJoin.PlayerCount++;
            team = toJoin;
        }
        else
        {
            Debug.LogError("No teams available!");
        }

        // Increment player count
        m_playerCount++;

        // Sync player colour and score
        for (int i = 0; i < 8; i++)
        {
            if (m_players[i] != null)
            {
                m_players[i].SetInitialColor(GetColor(m_players[i].Team.Id));
                m_players[i].SyncScore();
            }
        }

        // Set player colour for new player
        player.SetInitialColor(GetColor(team.Id));
        player.GetComponent<Identity>().SetIdentity();

        if (OnPlayersChange != null)
        {
            OnPlayersChange();
        }

        return team;
    }

    public void RemovePlayer(Soldier player)
    {
        // Decrease the playercount in the team the player was in
        Teams.Find(e => e == player.Team).PlayerCount--;

        // Decrease total player count
        m_playerCount--;

        // Remove player from list of players
        Soldier[] tempArray = new Soldier[8];
        int index = 0;
        // Fix the player array so that there are no gaps
        for (int i = 0; i < 8; i++)
        {
            if (m_players[i] != null && m_players[i] != player)
            {
                tempArray[index] = m_players[i];
                index++;
            }
        }

        m_players = tempArray;
        if (OnPlayersChange != null)
        {
            OnPlayersChange();
        }
    }

    public Soldier[] GetPlayers()
    {
        return m_players;
    }

    public Color GetColor(int teamId)
    {
        // Find the team with corresponding team id
        Team team = Teams.Find(e => e.Id == teamId);
        if (team != null)
        {
            return team.Color;
        }

        return new Color(0, 0, 0, 0);
    }

    public void SetTeamScore(int teamId, Score score)
    {
        Teams[teamId - 1].Score = score;
    }

    public void AddTeam()
    {
        Teams.Add(new Team(Teams.Count + 1));
    }

    public void RemoveTeam(Team team)
    {
        Teams.Remove(team);
        // Fix team id's so there are no gaps
        for (int i = 0; i < Teams.Count; i++)
        {
            Teams[i].Id = i;
        }
    }

    public Soldier[] GetAllPlayers()
    {
        return m_players;
    }
}
