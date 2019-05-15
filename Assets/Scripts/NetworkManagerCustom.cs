using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{
    private bool m_isConnecting;
    private string m_connectionText;

    // Instantiate the NetworkManager when the game starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        Object obj = Resources.Load("NetworkManager");
        GameObject networkManager = Instantiate(obj) as GameObject;
        // Rename NetworkManager(Clone) to NetworkManager
        networkManager.name = obj.name;
    }

    public override void Start()
    {
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
            string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
            if (ipAddress.Length == 0)
            {
                m_connectionText = "IP address must not be empty";
                return;
            }

            m_isConnecting = true;
            m_connectionText = "Connecting...";

            SetIPAddress(ipAddress);
            StartClient();
        }
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        // If the client is not properly disconnected...
        if ((UnityEngine.Networking.NetworkError)errorCode != UnityEngine.Networking.NetworkError.Ok)
        {
            // and if it is a timeout error, print "impossible to connect"
            if ((UnityEngine.Networking.NetworkError)errorCode == UnityEngine.Networking.NetworkError.Timeout)
            {
                m_connectionText = "Connection failed";
            }
            Debug.LogError("ClientDisconnected due to error: " + (UnityEngine.Networking.NetworkError)errorCode);
        }

        Debug.Log("Client disconnected from server: " + conn);
        m_isConnecting = false;
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // Remove disconnecting player from team
        GameObject.Find("GameManager").GetComponent<GameManager>().RemovePlayer(conn.playerController.gameObject.GetComponent<Player>());
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        // Instantiate new player and assign a team
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        GameObject.Find("GameManager").GetComponent<GameManager>().AddPlayer(player.GetComponent<Player>());
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        m_connectionText = "";
        m_isConnecting = false;
    }

    // Set the IP address of the network manager for the StartClient function
    private void SetIPAddress(string ipAddress)
    {
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