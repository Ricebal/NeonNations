using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BotManager : NetworkBehaviour
{
    // Prefab representing the bot
    public GameObject Bot;
    // Start is called before the first frame update
    void Start()
    {
        if (!isServer)
        {
            return;
        }
        GameObject bot = Instantiate(Bot, new Vector3(2, 0, 17), new Quaternion(0, 0, 0, 0));
        NetworkServer.Spawn(bot);
    }
}
