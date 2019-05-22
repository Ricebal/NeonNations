using System.Text;
using TMPro;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
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
    [SyncVar] [SerializeField] private int m_outerWallWidth = 14;

    private BoardManager m_boardManager;
    private BotManager m_botManager;
    private TeamManager m_teamManager;

    void Awake()
    {
        m_boardManager = GetComponent<BoardManager>();
        m_teamManager = GetComponent<TeamManager>();
        m_botManager = GetComponent<BotManager>();
    }

    private void Start()
    {
        InitGame();
    }

    void InitGame()
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
            m_maxRoomLength, m_minTunnelLength, m_maxTunnelLength, m_tunnelWidth, m_breakableTunnelChance);
        m_boardManager.SetupScene(mapGenerator.GenerateRandomMap(m_seed), m_outerWallWidth);
        m_botManager.SetupBots();
    }

    public void AddPlayer(Soldier player)
    {
        player.Team = m_teamManager.AddPlayer(player);
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
            ch = (char)Random.Range('a', 'z');
            builder.Append(ch);
        }
        return builder.ToString();
    }
}
