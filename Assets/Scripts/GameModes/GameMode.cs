using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameModes { TeamDeathMatch };
public abstract class GameMode: NetworkBehaviour
{
    public const string WIN = "Victory";
    public const string LOSE = "Lost";
    public const string DRAW = "Draw";
    // List of colors just so there are some presets. This might either be expanded or be deleted if it becomes unnessecary with the color-picker.
    public List<Color> Colors = new List<Color> { new Color32(10, 255, 0, 0), new Color32(255, 0, 0, 0), new Color32(241, 0, 204, 0) };
    /// <summary>
    /// The score needed to win the game.
    /// </summary>
    private int m_winCondition;
    public int AmountOfTeams;
    public int AmountOfPlayers;
    /// <summary>
    /// The timelimit in seconds
    /// </summary>
    [SyncVar] private float m_timeLimit;
    public delegate void OnScoreChangeDelegate();
    public event OnScoreChangeDelegate OnGameFinished;

    private TeamManager m_teamManager;
    public GameModes CurrentGameMode;

    public GameMode(GameModes gameMode, int winCondition, int amountOfTeams, int amountOfPlayers, float timeLimit)
    {
        CurrentGameMode = gameMode;
        m_winCondition = winCondition;
        AmountOfTeams = amountOfTeams;
        AmountOfPlayers = amountOfPlayers;
        m_timeLimit = timeLimit;
    }

    private void FixedUpdate()
    {
        CheckForGameTimedOut(Time.deltaTime);
    }
    public void SetTeamManager(TeamManager tm)
    {
        m_teamManager = tm;
    }
    
    public void CheckForWinCondition()
    {
        if (GameManager.GameFinished) // Don't change the score after the game has finished.
        {
            return;
        }
        foreach (Team team in m_teamManager.Teams)
        {
            if (team.Score.GetScore(CurrentGameMode) >= m_winCondition) // A team has met the win condition.
            {
                if(OnGameFinished != null) 
                {
                    OnGameFinished();
                }
            }
        }
    }

    private void CheckForGameTimedOut(float deltaTime)
    {
        m_timeLimit -= deltaTime;
        if(m_timeLimit > 0)
        {
            return; // The game isn't finished yet.
        }

        OnGameFinished(); // The game is finished.
    }

    public string RemainingTimeAsText()
    {
        return TimeSpan.FromSeconds(m_timeLimit).ToString(@"mm\:ss");
    }
}
