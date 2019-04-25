using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnBots : NetworkBehaviour
{
    // Prefab representing the bot
    public GameObject Bot;
    // Start is called before the first frame update
    void Start()
    {
        GameObject bot = Instantiate(Bot, new Vector3(13, 0, 13), new Quaternion(0, 0, 0, 0));
        NetworkServer.Spawn(bot);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
