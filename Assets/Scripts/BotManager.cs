using UnityEngine;
using Mirror;

public class BotManager : NetworkBehaviour
{
    // Prefab representing the bot
    public GameObject Bot;

    public void SetupBots()
    {
        if (!isServer)
        {
            return;
        }
        Vector2Int spawnPoint = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
        GameObject bot = Instantiate(Bot, new Vector3(spawnPoint.x, 0, spawnPoint.y), Quaternion.identity);
        NetworkServer.Spawn(bot);
        GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayer(bot.GetComponent<Bot>());
    }
}
