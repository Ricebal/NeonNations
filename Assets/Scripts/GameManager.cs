using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public string Seed = "";
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
        if (isServer && string.IsNullOrEmpty(Seed))
        {
            // set seed to be used by generation
            Seed = GenerateSeed();
        }

        // Display seed on the hud
        GameObject hud = GameObject.FindGameObjectWithTag("HUD");
        TextMeshProUGUI text = hud.GetComponent<TextMeshProUGUI>();
        text.text = Seed;

        MapGenerator mapGenerator = new MapGenerator();
        m_boardManager.SetupScene(mapGenerator.GenerateRandomMap(Seed));
        m_botManager.SetupBots();
    }

    public void AddPlayer(Soldier player)
    {
        player.Team = m_teamManager.AddPlayer(player);
    }

    public void RemovePlayer(Soldier player)
    {
        m_teamManager.RemovePlayer(player);
    }



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
}
