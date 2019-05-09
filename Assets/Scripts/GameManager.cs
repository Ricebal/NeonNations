using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour
{
    [SyncVar]
    public string Seed;
    public BoardManager BoardScript;
    public BotManager BotManager;

    void Start()
    {
        BoardScript = GetComponent<BoardManager>();
        BotManager = GetComponent<BotManager>();
        InitGame();
    }

    void InitGame()
    {
        if (isServer)
        {
            // set seed to be used by generation
            Seed = BoardScript.GenerateSeed();
        }
        BoardScript.SetupScene(Seed);
        BotManager.SetupBots();
    }
}
