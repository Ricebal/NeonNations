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
    public int WinCondition;
    public int AmountOfTeams;
    public int AmountOfPlayers;
    /// <summary>
    /// The timelimit in seconds
    /// </summary>
    [SyncVar] public float TimeLimit;
    public delegate void OnScoreChangeDelegate();
    public event OnScoreChangeDelegate OnGameFinished;

    private TeamManager m_teamManager;
    public GameModes CurrentGameMode;


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
            if (team.Score.GetScore(CurrentGameMode) >= WinCondition) // A team has met the win condition.
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
        TimeLimit -= deltaTime;
        if(TimeLimit > 0)
        {
            return; // The game isn't finished yet.
        }

        OnGameFinished(); // The game is finished.
    }

    public string FormatTimeToText()
    {
        if(TimeLimit < 0)
        {
            return "00:00";
        }
        string result = "";
        double minutes = Math.Floor(TimeLimit / 60);
        result += ZeroPrefixForTime(minutes);
        result += ":";
        double seconds = Math.Floor(TimeLimit % 60);
        result += ZeroPrefixForTime(seconds);
        return result;
    }

    private string ZeroPrefixForTime(double number)
    {
        string result = "";
        if (number < 10)
        {
            result += "0"; // Add the 0 for 00:03 or 02:06
        }
        result += number;
        return result;
    }
}
