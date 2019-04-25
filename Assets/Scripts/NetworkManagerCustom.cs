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

        //connectionConfig.MaxConnectionAttempt = 2;
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
            m_isConnecting = true;
            m_connectionText = "Connecting...";

            SetIPAddress();
            SetPort();
            StartClient();
        }
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        // If the client is not properly disconnected...
        if ((int) UnityEngine.Networking.NetworkError.Ok != errorCode)
        {
            // and if it is a timeout error, print "connection failed"
            if ((int) UnityEngine.Networking.NetworkError.Timeout == errorCode)
            {
                m_connectionText = "Connection failed";
            }
            // print the error in the console
            Debug.LogError("ClientDisconnected due to error: " + (UnityEngine.Networking.NetworkError) errorCode);
        }

        Debug.Log("Client disconnected from server: " + conn);
        m_isConnecting = false;
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

    // Set the port of the network manager for the StartClient function
    void SetPort()
    {
       // NetworkManager.singleton.networkPort = 7777;
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
