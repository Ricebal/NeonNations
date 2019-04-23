using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{
    public float TimeOut = 5;

    private bool m_isConnecting;
    private string m_connectionText;

    void Start() {
        m_isConnecting = false;
        m_connectionText = "";
    }

    // Start a game as a host
    public void StartUpHost() {
        if (m_isConnecting) {
            m_connectionText = "Disconnecting";
            StopClient();
        }
        SetPort();
        StartHost();
    }

    // Join a game as a client
    public void JoinGame() {
        if (!m_isConnecting) {
            m_isConnecting = true;
            m_connectionText = "Connecting...";

            SetIPAddress();
            SetPort();
            StartClient();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        StopClient();

        // If the client is not properly disconnected...
        if (conn.lastError != NetworkError.Ok) {
            // and if it is a timeout error, print "impossible to connect"
            if (conn.lastError == NetworkError.Timeout) {
                m_connectionText = "Impossible to connect.";
            }
            // otherwise print the error in the console
            if (LogFilter.logError) {
                Debug.LogError("ClientDisconnected due to error: " + conn.lastError);
            }
        }

        Debug.Log("Client disconnected from server: " + conn);
        m_isConnecting = false;
    }

    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        m_connectionText = "";
        m_isConnecting = false;
    }

    // Set the IP address of the network manager for the StartClient function
    void SetIPAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    // Set the port of the network manager for the StartClient function
    void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    public bool IsConnecting() {
        return m_isConnecting;
    }

    public string GetConnectionText() {
        return m_connectionText;
    }

    public void Stop() {
        StopClient();
        StopHost();
        m_isConnecting = false;
        m_connectionText = "";
    }
}
