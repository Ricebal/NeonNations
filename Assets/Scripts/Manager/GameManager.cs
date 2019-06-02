using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Singleton;

    public GameMode GameMode;
    public bool GameFinished;
    public float WaitingTimeAfterGameEnded; // The time after the game is finished, before it will return to the lobby.

    [SyncVar] [SerializeField] private string m_seed = "";
    [SyncVar] [SerializeField] private int m_mapWidth = 50;
    [SyncVar] [SerializeField] private int m_mapHeight = 50;
    [SyncVar] [SerializeField] private int m_maxRoomAmount = 100;
    [SyncVar] [SerializeField] private int m_maxShortcutAmount = 10;
    [SyncVar] [SerializeField] private int m_minRoomLength = 6;
    [SyncVar] [SerializeField] private int m_maxRoomLength = 9;
    [SyncVar] [SerializeField] private int m_minTunnelLength = 1;
    [SyncVar] [SerializeField] private int m_maxTunnelLength = 7;
    [SyncVar] [SerializeField] private int m_tunnelWidth = 2;
    [SyncVar] [SerializeField] private int m_breakableTunnelChance = 20;
    [SyncVar] [SerializeField] private int m_shortcutMinSkipDistance = 20;
    [SyncVar] [SerializeField] private int m_reflectorAreaSize = 200;
    [SyncVar] [SerializeField] private int m_outerWallWidth = 14;
    [SerializeField] private ParticleSystem m_fireWorks;

    private GameObject m_endGameTextObject;
    private int m_localPlayersTeamId;

    private void Awake()
    {
        InitializeSingleton();
        GameMode = gameObject.AddComponent<TeamDeathMatch>(); // Temporary untill we can pick game modes.
        SetTeams();
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
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        WaitingTimeAfterGameEnded = 6; // 6 seconds.
        m_localPlayersTeamId = 0;
        GameFinished = false;
    }

    private void Start()
    {
        if (isServer)
        {
            GameMode.OnGameFinished += FinishGame;
        }
        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        m_endGameTextObject = hud.transform.Find("EndGameText").gameObject;
        GameFinished = false;
        InitGame();
    }

    private void FixedUpdate()
    {
        if (!GameFinished)
        {
            return;
        }
        WaitingTimeAfterGameEnded -= Time.deltaTime;
        if (WaitingTimeAfterGameEnded < 3) // For first 3 seconds show endgame text.
        {
            m_endGameTextObject.SetActive(false); // Hide endgame text.
        }
        if (WaitingTimeAfterGameEnded < 0) // After the delay, go back to lobby.
        {
            if (isServer)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
            SceneManager.LoadScene("NetworkMenu", LoadSceneMode.Single);
        }
    }

    private void SetTeams()
    {
        for (int i = 0; i < GameMode.AmountOfTeams; i++) // Add the teams
        {
            try
            {
                TeamManager.AddTeam(GameMode.Colors[i]);
            }
            catch (Exception) // For if you want to add more teams than in the List. The list is just temperary untill we can pick teamcolors.
            {
                TeamManager.AddTeam(new Color(0, 0, 1, 1));
            }
        }
    }

    private void InitGame()
    {
        if (isServer && string.IsNullOrEmpty(m_seed))
        {
            // set seed to be used by generation
            m_seed = GenerateSeed();
        }

        // Display seed on the debug mode
        DebugMode.SetSeed(m_seed);

        MapGenerator mapGenerator = new MapGenerator(m_mapWidth, m_mapHeight, m_maxRoomAmount, m_maxShortcutAmount, m_minRoomLength,
            m_maxRoomLength, m_minTunnelLength, m_maxTunnelLength, m_tunnelWidth, m_breakableTunnelChance, m_shortcutMinSkipDistance, m_reflectorAreaSize);
        BoardManager.SetupScene(mapGenerator.GenerateRandomMap(m_seed), m_outerWallWidth);
        BotManager.SetupBots();
    }

    [ClientRpc]
    private void RpcSetLocalPlayerTeamId(int teamId)
    {
        if (m_localPlayersTeamId == 0) // Only set the team id if it's not set yet.
        {
            m_localPlayersTeamId = teamId;
        }
    }

    public static void AddPlayer(Soldier player)
    {
        TeamManager.SyncTeams();
        TeamManager.AddPlayer(player);
        if (player as Player)
        {
            Singleton.RpcSetLocalPlayerTeamId(player.Team.Id);
        }
    }

    public static void RemovePlayer(Soldier player) => TeamManager.RemovePlayer(player);

    /// <summary>
    /// Generate a random seed
    /// </summary>
    /// <returns>A string containing the random seed</returns>
    private string GenerateSeed()
    {
        StringBuilder builder = new StringBuilder();
        char ch;
        for (int i = 0; i < 20; i++)
        {
            ch = (char)UnityEngine.Random.Range('a', 'z');
            builder.Append(ch);
        }
        return builder.ToString();
    }

    public void FinishGame()
    {
        RpcFinishGame();
    }

    [ClientRpc]
    public void RpcFinishGame()
    {
        GameObject[] soldierGameObjects = GameObject.FindGameObjectsWithTag("Player");
        Soldier localPlayer = new Player();
        foreach (GameObject go in soldierGameObjects)
        {
            Soldier soldier = go.GetComponent<Soldier>();
            if (soldier != null)
            {
                soldier.DisableMovement();
                Player player = soldier.GetComponent<Player>();
                if (player != null && player.isLocalPlayer) // Set the local player.
                {
                    localPlayer = soldier;
                }
            }
        }
        List<int> winningTeamIds = GetWinningTeams();
        GameFinished = true;
        bool draw = (winningTeamIds.Count > 1); // If there are more winning teams, it will be a draw.

        // Get teamcolor.
        Color teamColor = TeamManager.Singleton.Teams.Find(t => t.Id == m_localPlayersTeamId).Color;
        // Set end game text visible.
        m_endGameTextObject.SetActive(true);
        TextMeshProUGUI endGameText = m_endGameTextObject.GetComponent<TextMeshProUGUI>();
        if (winningTeamIds.Contains(m_localPlayersTeamId)) // If the team is winning.
        {
            if (draw)
            {
                // Go to draw screen.
                endGameText.text = GameMode.DRAW;
            }
            else
            {
                // Go to win screen.
                endGameText.text = GameMode.WIN;
                SetFireWorkColor(teamColor);
                SpawnFireWorks(localPlayer);
            }
        }
        else
        {
            // Go to lose screen.
            endGameText.text = GameMode.LOSE;
        }

        // Set color of text.
        endGameText.outlineColor = teamColor;
        Material mat = Material.Instantiate(endGameText.fontSharedMaterial);
        Color32 teamColor32 = teamColor;
        // Set alpha because that's not saved in team.Color.
        teamColor32.a = 255;
        mat.SetColor(ShaderUtilities.ID_GlowColor, teamColor32);
        endGameText.fontSharedMaterial = mat;
    }

    private void SetFireWorkColor(Color color)
    {
        var main = m_fireWorks.main;
        main.startColor = color;

        ParticleSystemRenderer[] particleSystemRenderers = m_fireWorks.GetComponentsInChildren<ParticleSystemRenderer>();
        for (int i = 0; i < particleSystemRenderers.Length; i++)
        {
            if (particleSystemRenderers[i].trailMaterial != null)
            {
                Material mat = Material.Instantiate(particleSystemRenderers[i].trailMaterial);
                mat.SetColor("_EmissionColor", color * 3);
                particleSystemRenderers[i].trailMaterial = mat;
            }
        }
    }

    private void SpawnFireWorks(Soldier soldier)
    {
        int amountOfFirworks = UnityEngine.Random.Range(5, 7);
        for (int i = 0; i < amountOfFirworks; i++)
        {
            float xPos = soldier.gameObject.transform.position.x;
            float zPos = soldier.gameObject.transform.position.z;
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(xPos - 7, xPos + 7), 1, UnityEngine.Random.Range(zPos - 5, zPos + 5));
            ParticleSystem ps = Instantiate(m_fireWorks, spawnPosition, Quaternion.identity);
            ps.Simulate(1);
            ps.Play();
        }
    }

    private List<int> GetWinningTeams()
    {
        List<int> winningTeamIds = new List<int>();
        int maxScore = 0;

        //Check for the team with the highest score.
        foreach (Team team in TeamManager.Singleton.Teams)
        {
            int score = GameMode.CalculateScore(team.Score);
            if (score > maxScore)   // New highest score is found.
            {
                winningTeamIds.Clear();
                winningTeamIds.Add(team.Id);
                maxScore = score;
            }
            else if (score == maxScore) // There might be a draw.
            {
                winningTeamIds.Add(team.Id);
            }
        }
        return winningTeamIds;
    }
}
