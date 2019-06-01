public class TeamDeathMatch : GameMode
{
    public TeamDeathMatch() : base(10, 2, 6, 300) { }// 300 = 5 minutes

    /// <summary>
    /// Create custom deathmatch
    /// </summary>
    /// <param name="kills">Amount of kills to win the game.</param>
    /// <param name="teams">Amount of teams in the game.</param>
    /// <param name="players">Amount of players in the game.</param>
    /// <param name="timeLimit">Length of the game in seconds</param>
    public TeamDeathMatch(int kills, int teams, int players, int timeLimit) : base(kills, teams, players, timeLimit) { }

    public override int CalculateScore(Score score)
    {
        return score.Kills;
    }
}
