using Mirror;
using UnityEngine;

public class BotManager : NetworkBehaviour
{
    public static BotManager Singleton;

    private const int AMOUNT_OF_BOTS = 4;
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

        for (int i = 0; i < AMOUNT_OF_BOTS; i++)
        {
            GameObject bot = Instantiate(Singleton.Bot, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(bot);
            GameManager.AddPlayer(bot.GetComponent<Soldier>());
        }
    }
}
