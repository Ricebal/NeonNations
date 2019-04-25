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

    public int AddPlayer()
    {
        int team = 0;

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

        m_playerCount++;

        // Debug.Log("Player joined team: " + team + "\n Team 1: " + m_team1 + "\n Team 2: " + m_team2 + "\n Total: " + m_playerCount);

        return team;
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
