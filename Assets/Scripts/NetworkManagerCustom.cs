using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{
    private bool m_isConnecting;
    private string m_connectionText;

    public override void Start()
    {
        base.Start();

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
        StartHost();
    }

    // Join a game as a client
    public void JoinGame()
    {
        if (!m_isConnecting)
        {
            m_isConnecting = true;
            m_connectionText = "Connecting...";

            SetIPAddress();
            StartClient();
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        // If the client is disconnected during a connection attempt, an error occurred
        if(m_isConnecting)
        {
            m_connectionText = "An error occurred while connecting";
            m_isConnecting = false;
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        m_connectionText = "";
        m_isConnecting = false;
    }

    // Set the IP address of the network manager for the StartClient function
    void SetIPAddress()
    {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
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
