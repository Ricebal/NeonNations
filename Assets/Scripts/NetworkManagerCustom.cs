﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{
    private bool m_isConnecting;
    private string m_connectionText;
    private GameManager m_gameManager;

    void Start()
    {
        connectionConfig.MaxConnectionAttempt = 2;
        m_isConnecting = false;
        m_connectionText = "";
    }

    // Start a game as a host
    public void StartUpHost()
    {
        if (m_isConnecting)
        {
            m_connectionText = "Disconnecting";
            StopClient();
        }
        SetPort();
        StartHost();
    }

    // Join a game as a client
    public void JoinGame()
    {
        if (!m_isConnecting)
        {
            string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
            if (ipAddress.Length == 0)
            {
                m_connectionText = "IP address must not be empty";
                return;
            }

            m_isConnecting = true;
            m_connectionText = "Connecting...";

            SetIPAddress(ipAddress);
            SetPort();
            StartClient();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        StopClient();

        // If the client is not properly disconnected...
        if (conn.lastError != NetworkError.Ok)
        {
            // and if it is a timeout error, print "impossible to connect"
            if (conn.lastError == NetworkError.Timeout)
            {
                m_connectionText = "Connection failed";
            }
            // otherwise print the error in the console
            if (LogFilter.logError)
            {
                Debug.LogError("ClientDisconnected due to error: " + conn.lastError);
            }
        }

        Debug.Log("Client disconnected from server: " + conn);
        m_isConnecting = false;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayer(player.GetComponent<Player>());
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        m_connectionText = "";
        m_isConnecting = false;
    }

    // Set the IP address of the network manager for the StartClient function
    void SetIPAddress(string ipAddress)
    {
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    // Set the port of the network manager for the StartClient function
    void SetPort()
    {
        NetworkManager.singleton.networkPort = 7777;
    }

    public bool IsConnecting()
    {
        return m_isConnecting;
    }

    public string GetConnectionText()
    {
        return m_connectionText;
    }

    public void Stop()
    {
        StopClient();
        StopHost();
        m_isConnecting = false;
        m_connectionText = "";
    }
}
