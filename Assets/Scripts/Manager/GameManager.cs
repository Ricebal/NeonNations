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
    [SyncVar] private int m_mapWidth;
    [SyncVar] private int m_mapHeight;
    [SyncVar] private int m_maxRoomAmount;
    [SyncVar] private int m_maxShortcutAmount;
    [SyncVar] private int m_minRoomLength;
    [SyncVar] private int m_maxRoomLength;
    [SyncVar] private int m_minTunnelLength;
    [SyncVar] private int m_maxTunnelLength;
    [SyncVar] private int m_tunnelWidth;
    [SyncVar] private int m_breakableTunnelChance;
    [SyncVar] private int m_shortcutMinSkipDistance;
    [SyncVar] private int m_reflectorAreaSize;
    [SyncVar] [SerializeField] private int m_outerWallWidth = 14;

    [SerializeField] private ParticleSystem m_fireworks = null;
    [SerializeField] private GameObject m_endGameTextObject;
    private int m_localPlayersTeamId;

    private void Awake()
    {
        InitializeSingleton();
        InitializeVariables();
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
            InitMap();
            GameMode.OnGameFinished += FinishGame;
        }
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
            SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
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

    private void InitMap()
    {
        m_mapWidth = LobbyConfigMenu.GetOptionValue("Map width");
        m_mapHeight = LobbyConfigMenu.GetOptionValue("Map height");
        m_maxRoomAmount = LobbyConfigMenu.GetOptionValue("Max room amount");
        m_maxShortcutAmount = LobbyConfigMenu.GetOptionValue("Max shortcut amount");
        m_minRoomLength = LobbyConfigMenu.GetOptionValue("Min room length");
        m_maxRoomLength = LobbyConfigMenu.GetOptionValue("Max room length");
        m_minTunnelLength = LobbyConfigMenu.GetOptionValue("Min tunnel length");
        m_maxTunnelLength = LobbyConfigMenu.GetOptionValue("Max tunnel length");
        m_tunnelWidth = LobbyConfigMenu.GetOptionValue("Tunnel width");
        m_breakableTunnelChance = LobbyConfigMenu.GetOptionValue("Breakable tunnel chance");
        m_shortcutMinSkipDistance = LobbyConfigMenu.GetOptionValue("Shortcut min skip distance");
        m_reflectorAreaSize = LobbyConfigMenu.GetOptionValue("Reflector area size");
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
                soldier.StopMovement();
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
                SpawnFireworks(localPlayer);
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
        var main = m_fireworks.main;
        main.startColor = color;

        ParticleSystemRenderer[] particleSystemRenderers = m_fireworks.GetComponentsInChildren<ParticleSystemRenderer>();
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

    private void SpawnFireworks(Soldier soldier)
    {
        int amountOfFireworks = UnityEngine.Random.Range(5, 7);
        for (int i = 0; i < amountOfFireworks; i++)
        {
            float xPos = soldier.gameObject.transform.position.x;
            float zPos = soldier.gameObject.transform.position.z;
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(xPos - 7, xPos + 7), 1, UnityEngine.Random.Range(zPos - 5, zPos + 5));
            ParticleSystem ps = Instantiate(m_fireworks, spawnPosition, Quaternion.identity);
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
            if (score > maxScore) // New highest score is found.
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
