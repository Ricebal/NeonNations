using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDeathMatch: GameMode
{
    public TeamDeathMatch()
    {
        CurrentGameMode = GameModes.TeamDeathMatch;
        WinCondition = 2;
        AmountOfTeams = 2;
        AmountOfPlayers = 6;
        TimeLimit = 60;// 300; // 5 minutes
    }

    /// <summary>
    /// Create custom deathmatch
    /// </summary>
    /// <param name="kills">Amount of kills to win the game.</param>
    /// <param name="teams">Amount of teams in the game.</param>
    /// <param name="players">Amount of players in the game.</param>
    /// <param name="timeLimit">How long the match should take.</param>
    public TeamDeathMatch(int kills, int teams, int players, int timeLimit)
    {
        CurrentGameMode = GameModes.TeamDeathMatch;
        WinCondition = kills;
        AmountOfTeams = teams;
        AmountOfPlayers = players;
        TimeLimit = timeLimit;
    }
}
