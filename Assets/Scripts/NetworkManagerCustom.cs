using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{

    // Start a game as a host
    public void StartUpHost() {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    // Join a game as a client
    public void JoinGame() {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    // Set the IP address of the network manager for the StartClient function
    void SetIPAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

}
