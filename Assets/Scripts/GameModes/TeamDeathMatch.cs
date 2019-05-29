using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDeathMatch: GameMode
{
    public TeamDeathMatch(): base(GameModes.TeamDeathMatch, 10, 2, 6, 300) // 300 = 5 minutes
    {
    }

    /// <summary>
    /// Create custom deathmatch
    /// </summary>
    /// <param name="kills">Amount of kills to win the game.</param>
    /// <param name="teams">Amount of teams in the game.</param>
    /// <param name="players">Amount of players in the game.</param>
    /// <param name="timeLimit">Length of the game in seconds</param>
    public TeamDeathMatch(int kills, int teams, int players, int timeLimit): base(GameModes.TeamDeathMatch, kills, teams, players, timeLimit)
    {
    }
}
