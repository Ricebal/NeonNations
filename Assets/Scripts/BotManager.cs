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
        Vector2 spawnPoint = GameObject.Find("GameManager").GetComponent<BoardManager>().GetRandomFloorTile();
        GameObject bot = Instantiate(Bot, new Vector3(spawnPoint.x, 0, spawnPoint.y), new Quaternion(0, 0, 0, 0));
        NetworkServer.Spawn(bot);
    }
}
