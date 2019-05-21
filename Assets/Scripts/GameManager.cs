using Mirror;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public string Seed;
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
        if (isServer)
        {
            // set seed to be used by generation
            Seed = m_boardManager.GenerateSeed();
        }
        m_boardManager.SetupScene(Seed);
        m_botManager.SetupBots();
    }

    public void AddPlayer(Soldier player)
    {
        player.Team = m_teamManager.AddPlayer(player);
    }

    public void RemovePlayer(Soldier player) => m_teamManager.RemovePlayer(player);
}
