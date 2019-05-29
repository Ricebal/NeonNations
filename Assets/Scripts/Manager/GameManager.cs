using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text;
using Mirror;
using TMPro;
using UnityEngine;
using System;

public class GameManager : NetworkBehaviour
{
    public GameMode GameMode;
    public static bool GameFinished;

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
    [SyncVar] [SerializeField] private int m_outerWallWidth = 14;
    [SerializeField] private ParticleSystem m_fireWorks;

    private BoardManager m_boardManager;
    private BotManager m_botManager;
    private TeamManager m_teamManager;
    private GameObject m_endGameTextObject;
    private float m_timeAfterFinishingTheGame = 3;
    private int m_localPlayersTeamId = 0;

    void Awake()
    {
        m_boardManager = GetComponent<BoardManager>();
        GameMode = gameObject.AddComponent<TeamDeathMatch>(); // Temporary untill we can pick game modes.
        m_teamManager = GetComponent<TeamManager>();
        SetTeams();
        m_botManager = GetComponent<BotManager>();
    }

    private void Start()
    {
        if (isServer)
        {
            GameMode.OnGameFinished += FinishGame;
        }
        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        m_endGameTextObject = hud.transform.Find("EndGameText").gameObject;
        GameMode.SetTeamManager(m_teamManager);
        GameFinished = false;
        InitGame();
    }

    private void FixedUpdate()
    {
        if (!GameFinished)
        {
            return;
        }
        m_timeAfterFinishingTheGame -= Time.deltaTime;
        if(m_timeAfterFinishingTheGame <= 0)
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
        for(int i = 0; i < GameMode.AmountOfTeams; i++) // Add the teams
        {
            try
            {
                m_teamManager.AddTeam(GameMode.Colors[i]);
            }
            catch (Exception e) // For if you want to add more teams than in the List. The list is just temperary untill we can pick teamcolors.
            {
                m_teamManager.AddTeam(new Color(0, 0, 1, 1));
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

        // Display seed on the hud
        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        TextMeshProUGUI text = hud.GetComponent<TextMeshProUGUI>();
        text.text = m_seed;

        MapGenerator mapGenerator = new MapGenerator(m_mapWidth, m_mapHeight, m_maxRoomAmount, m_maxShortcutAmount, m_minRoomLength,
            m_maxRoomLength, m_minTunnelLength, m_maxTunnelLength, m_tunnelWidth, m_breakableTunnelChance, m_shortcutMinSkipDistance);
        m_boardManager.SetupScene(mapGenerator.GenerateRandomMap(m_seed), m_outerWallWidth);
        m_botManager.SetupBots();
    }

    [ClientRpc]
    private void RpcSetLocalPlayerTeamId(int teamId)
    {
        if(m_localPlayersTeamId == 0) // Only set the team id if it's not set yet.
        {
            m_localPlayersTeamId = teamId;
        }
    }

    public void AddPlayer(Soldier player)
    {
        m_teamManager.SyncTeams();
        m_teamManager.AddPlayer(player);
        if(player as Player)
        {
            RpcSetLocalPlayerTeamId(player.Team.Id);
        }
    }

    public void RemovePlayer(Soldier player) => m_teamManager.RemovePlayer(player);

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

    /// <summary>
    /// Will regulate which player will see the victory, losing or draw-screen.
    /// </summary>
    /// <param name="teamIds">The ids of the winning teams</param>
    [ClientRpc]
    public void RpcFinishGame()
    {
        GameObject[] soldierGameObjects = GameObject.FindGameObjectsWithTag("Player");
        List<Soldier> soldiers = new List<Soldier>();
        foreach (GameObject go in soldierGameObjects)
        {
            Soldier soldier = go.GetComponent<Soldier>();
            if(soldier != null)
            {
                soldier.DisableMovement();
                soldiers.Add(soldier);
            }
        }
        List<int> winningTeamIds = CheckWinningTeam();
        GameFinished = true;
        bool draw = false;
        if(winningTeamIds.Count > 1) // If there are more winning teams, it will be a draw.
        {
            draw = true;
        }
        // Get teamcolor.
        Color teamColor = m_teamManager.Teams.Find(t => t.Id == m_localPlayersTeamId).Color;
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
                //SetFireWorkColor(teamColor);
                //SpawnFireWorks(soldiers);
            }
        }
        else
        {
            // Go to lose screen.
            endGameText.text = GameMode.LOSE;
        }

        SetFireWorkColor(teamColor);
        SpawnFireWorks(soldiers);
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

    private void SpawnFireWorks(List<Soldier> soldiers)
    {
        foreach(Soldier soldier in soldiers)
        { 
            if(soldier.Team.Id == m_localPlayersTeamId)
            {
                for (int i = 0; i < 3; i++)
                {
                    float xPos = soldier.gameObject.transform.position.x;
                    float zPos = soldier.gameObject.transform.position.z;
                    Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(xPos - 7, xPos + 7), 1, UnityEngine.Random.Range(zPos - 5, zPos + 5));
                    ParticleSystem ps = Instantiate(m_fireWorks, spawnPosition, Quaternion.identity);
                    ps.Simulate(1);
                    ps.Play();  
                }
            }
        }
    }
    private List<int> CheckWinningTeam()
    {
        List<int> winningTeamIds = new List<int>();
        int maxScore = 0;

        //Check for the team with the highest score.
        foreach (Team team in m_teamManager.Teams)
        {
            int score = team.Score.GetScore(GameMode.CurrentGameMode);
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
