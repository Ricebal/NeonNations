using Mirror;
using UnityEngine;

public class BotManager : NetworkBehaviour
{
    public static BotManager Singleton;

    // Prefab representing the bot
    [SerializeField] private GameObject m_botPrefab = null;

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

        for (int i = 0; i < LobbyConfig.GetAmountOfBots(); i++)
        {
            GameObject bot = Instantiate(Singleton.m_botPrefab, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(bot);
            GameManager.AddPlayer(bot.GetComponent<Soldier>());
        }
    }
}
