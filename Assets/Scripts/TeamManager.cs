using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public Color ColorTeam1;
    public Color ColorTeam2;
    private int m_team1;
    private int m_team2;
    private int m_playerCount;
    private Soldier[] m_players = new Soldier[8];

    public int AddPlayer(Soldier player)
    {
        int team = 0;

        // Add player to list of players
        m_players[m_playerCount] = player;

        // Increment team
        if (m_team1 > m_team2)
        {
            team = 2;
            m_team2++;
        }
        else
        {
            team = 1;
            m_team1++;
        }

        // Increment player count
        m_playerCount++;

        // Set player colour for all existing players
        for (int i = 0; i < 8; i++)
        {
            if (m_players[i] != null)
            {
                m_players[i].SetInitialColor(GetColor(m_players[i].Team));
            }
        }

        // Set player colour for new player
        player.SetInitialColor(GetColor(team));

        return team;
    }

    public void RemovePlayer(Soldier player)
    {
        // Decrease team count
        if (player.Team == 1)
        {
            m_team1--;
        }
        else if (player.Team == 2)
        {
            m_team2--;
        }

        // Decrease player count
        m_playerCount--;

        // Remove player from list of players
        Soldier[] tempArray = new Soldier[8];
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            if (m_players[i] != null && m_players[i] != player)
            {
                tempArray[index] = m_players[i];
                index++;
            }
        }

        m_players = tempArray;
    }

    public Soldier[] GetPlayers()
    {
        return m_players;
    }

    public Color GetColor(int team)
    {
        switch (team)
        {
            case 1:
                return ColorTeam1;
            case 2:
                return ColorTeam2;
            default:
                return new Color(0, 0, 0, 0);
        }
    }
}
