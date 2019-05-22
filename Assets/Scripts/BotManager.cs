using Mirror;
using UnityEngine;

public class BotManager : NetworkBehaviour
{
    // Prefab representing the bot
    public GameObject Bot;
    private const int AMOUNT_OF_BOTS = 1;

    public void SetupBots()
    {
        if (!isServer)
        {
            return;
        }
        for (int i = 0; i < AMOUNT_OF_BOTS; i++)
        {
            Vector2Int spawnPoint = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
            GameObject bot = Instantiate(Bot, new Vector3(spawnPoint.x, 0, spawnPoint.y), Quaternion.identity);
            NetworkServer.Spawn(bot);
            GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayer(bot.GetComponent<Bot>());
        }
    }
}
