using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    public static TeamManager Singleton;
    public List<Team> Teams;
    private int m_playerCount;
    private Soldier[] m_players = new Soldier[8];

    public delegate void OnPlayersChangeDelegate();
    public event OnPlayersChangeDelegate OnPlayersChange;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    public static void AddPlayer(Soldier player)
    {
        Team team = new Team();

        // Add player to list of players
        Singleton.m_players[Singleton.m_playerCount] = player;

        // If there's no team throw a debug
        if (Singleton.Teams.Count > 0)
        {
            // Get the team with the least players
            Team toJoin = Singleton.Teams.Aggregate((min, next) => min.PlayerCount < next.PlayerCount ? min : next);
            toJoin.PlayerCount++;
            team = toJoin;
        }
        else
        {
            Debug.LogError("No teams available!");
        }

        // Increment player count
        Singleton.m_playerCount++;

        // Sync player colour and score
        for (int i = 0; i < 8; i++)
        {
            if (Singleton.m_players[i] != null)
            {
                Singleton.m_players[i].SetInitialColor(GetColor(Singleton.m_players[i].Team.Id));
                Singleton.m_players[i].SyncScore();
            }
        }

        // Set player colour for new player
        player.SetInitialColor(GetColor(team.Id));
        player.GetComponent<Identity>().SetIdentity();
        player.Team = team;

        Singleton.OnPlayersChange?.Invoke();
    }

    public static void RemovePlayer(Soldier player)
    {
        // Decrease the playercount in the team the player was in
        Singleton.Teams.Find(e => e.Id == player.Team.Id).PlayerCount--;

        // Decrease total player count
        Singleton.m_playerCount--;

        // Remove player from list of players
        Soldier[] tempArray = new Soldier[8];
        int index = 0;
        // Fix the player array so that there are no gaps
        for (int i = 0; i < 8; i++)
        {
            if (Singleton.m_players[i] != null && Singleton.m_players[i] != player)
            {
                tempArray[index] = Singleton.m_players[i];
                index++;
            }
        }

        Singleton.m_players = tempArray;
        Singleton.OnPlayersChange?.Invoke();
    }

    public static Soldier[] GetPlayers()
    {
        return Singleton.m_players;
    }

    public static Color GetColor(int teamId)
    {
        // Find the team with corresponding team id
        Team team = Singleton.Teams.Find(e => e.Id == teamId);
        if (team != null)
        {
            return team.Color;
        }

        return new Color(0, 0, 0, 0);
    }

    public static void SetTeamScore(int teamId, Score score)
    {
        Singleton.Teams[teamId - 1].Score = score;
    }

    public static void AddTeam(Color color)
    {
        Team team = new Team(Singleton.Teams.Count + 1);
        team.Color = color;
        Singleton.Teams.Add(team);
    }

    public static void AddTeam()
    {
        Singleton.Teams.Add(new Team(Singleton.Teams.Count + 1));
    }

    public static void RemoveTeam(Team team)
    {
        Singleton.Teams.Remove(team);
        int i = 0;
        // Fix team id's so there are no gaps
        Singleton.Teams.ForEach(e =>
        {
            i++;
            for (int j = 0; j < 8; j++)
            {
                // If there are players in a team fix played team id's
                if (Singleton.m_players[j] != null && Singleton.m_players[j].Team.Id == e.Id)
                {
                    Singleton.m_players[j].Team.Id = i;
                }
            }
            e.Id = i;
        });
    }

    public static void SyncTeams()
    {
        Singleton.Teams.ForEach(team =>
        {
            Singleton.RpcSyncTeamScore(team.Id, team.Score.Kills, team.Score.Deaths);
        });
    }

    [ClientRpc]
    private void RpcSyncTeamScore(int teamId, int kills, int deaths)
    {
        // Sync the score for all the teams.
        if (Singleton.isServer)   // Except for the server since that is the correct score.
        {
            return;
        }
        SetTeamScore(teamId, new Score(kills, deaths));
    }

    public static Soldier[] GetAllPlayers()
    {
        return Singleton.m_players;
    }

    public static List<Soldier> GetAliveEnemiesByTeam(int teamId)
    {
        List<Soldier> enemies = new List<Soldier>();
        foreach (Soldier player in Singleton.m_players)
        {
            if (player != null && player.Team.Id != teamId && !player.IsDead)
            {
                enemies.Add(player);
            }
        }
        return enemies;
    }
}
