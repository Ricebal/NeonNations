using Mirror;
using UnityEngine;

public class BotManager : NetworkBehaviour
{
    public static BotManager Singleton;

    private const int AMOUNT_OF_BOTS = 1;
    // Prefab representing the bot
    public GameObject Bot;

    private void Awake()
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

    public static void SetupBots()
    {
        if (!Singleton.isServer)
        {
            return;
        }
        for (int i = 0; i < BotManager.AMOUNT_OF_BOTS; i++)
        {
            Vector2Int spawnPoint = BoardManager.GetMap().GetSpawnPoint();
            GameObject bot = Instantiate(Singleton.Bot, new Vector3(spawnPoint.x, 0, spawnPoint.y), Quaternion.identity);
            NetworkServer.Spawn(bot);
            GameManager.AddPlayer(bot.GetComponent<Bot>());
        }
    }
}
