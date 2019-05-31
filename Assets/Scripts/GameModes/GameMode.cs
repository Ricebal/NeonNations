using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameMode : NetworkBehaviour
{
    public const string WIN = "Victory";
    public const string LOSE = "Lost";
    public const string DRAW = "Draw";
    // List of colors just so there are some presets. This might either be expanded or be deleted if it becomes unnessecary with the color-picker.
    public List<Color> Colors = new List<Color> { new Color32(10, 255, 0, 0), new Color32(255, 0, 0, 0), new Color32(241, 0, 204, 0) };
    public int AmountOfTeams;
    public int AmountOfPlayers;
    /// <summary>
    /// The score needed to win the game.
    /// </summary>
    private int m_winCondition;
    /// <summary>
    /// The timelimit in seconds
    /// </summary>
    [SyncVar] private float m_timeLimit;

    public delegate void OnGameFinishedDelegate();
    public event OnGameFinishedDelegate OnGameFinished;

    public GameMode(int winCondition, int amountOfTeams, int amountOfPlayers, float timeLimit)
    {
        m_winCondition = winCondition;
        AmountOfTeams = amountOfTeams;
        AmountOfPlayers = amountOfPlayers;
        m_timeLimit = timeLimit;
    }

    private void FixedUpdate()
    {
        CheckForGameTimedOut(Time.deltaTime);
    }

    public void CheckForWinCondition()
    {
        if (GameManager.Singleton.GameFinished) // Don't change the score after the game has finished.
        {
            return;
        }
        foreach (Team team in TeamManager.Singleton.Teams)
        {
            if (CalculateScore(team.Score) >= m_winCondition) // A team has met the win condition.
            {
                if (OnGameFinished != null)
                {
                    OnGameFinished();
                }
            }
        }
    }

    private void CheckForGameTimedOut(float deltaTime)
    {
        if (!GameManager.Singleton.GameFinished)
        {
            m_timeLimit -= deltaTime;
            if (m_timeLimit <= 0)
            {
                OnGameFinished(); // The game is finished.
            }
        }
    }

    /// <summary>
    /// Returns the score, depending on the gamemode.
    /// </summary>
    public abstract int CalculateScore(Score score);

    public string RemainingTimeAsText()
    {
        return TimeSpan.FromSeconds(m_timeLimit).ToString(@"mm\:ss");
    }
}
