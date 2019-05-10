using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        for (int i = 0; i < 20; i++)
        {
            Vector2 spawnPoint = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
            GameObject bot = Instantiate(Bot, new Vector3(spawnPoint.x, 0, spawnPoint.y), Quaternion.identity);
            NetworkServer.Spawn(bot);
            GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayer(bot.GetComponent<Bot>());
        }
    }
}
