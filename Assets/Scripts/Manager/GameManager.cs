using System.Text;
using Mirror;
using TMPro;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SyncVar][SerializeField] private string m_seed = "";
    [SyncVar][SerializeField] private int m_mapWidth = 50;
    [SyncVar][SerializeField] private int m_mapHeight = 50;
    [SyncVar][SerializeField] private int m_maxRoomAmount = 100;
    [SyncVar][SerializeField] private int m_maxShortcutAmount = 10;
    [SyncVar][SerializeField] private int m_minRoomLength = 6;
    [SyncVar][SerializeField] private int m_maxRoomLength = 9;
    [SyncVar][SerializeField] private int m_minTunnelLength = 1;
    [SyncVar][SerializeField] private int m_maxTunnelLength = 7;
    [SyncVar][SerializeField] private int m_tunnelWidth = 2;
    [SyncVar][SerializeField] private int m_breakableTunnelChance = 20;
    [SyncVar][SerializeField] private int m_shortcutMinSkipDistance = 20;
    [SyncVar][SerializeField] private int m_outerWallWidth = 14;

    public static GameManager Singleton;

    void Awake()
    {
        InitializeSingleton();
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
            m_maxRoomLength, m_minTunnelLength, m_maxTunnelLength, m_tunnelWidth, m_breakableTunnelChance, m_shortcutMinSkipDistance);
        BoardManager.SetupScene(mapGenerator.GenerateRandomMap(m_seed), m_outerWallWidth);
        BotManager.SetupBots();
    }

    public static void AddPlayer(Soldier player)
    {
        player.Team = TeamManager.AddPlayer(player);
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
            ch = (char)Random.Range('a', 'z');
            builder.Append(ch);
        }
        return builder.ToString();
    }
}
