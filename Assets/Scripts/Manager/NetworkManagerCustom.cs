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
            if (string.IsNullOrWhiteSpace(ipAddress))
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

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        m_connectionText = "";
        m_isConnecting = false;
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        UnityEngine.Networking.NetworkError error = (UnityEngine.Networking.NetworkError)errorCode;
        m_connectionText = "Connection failed due to error: " + error.ToString();
        m_isConnecting = false;
    }

    public override void OnStopClient()
    {
        if (m_isConnecting)
        {
            m_connectionText = "Connection timed out";
            m_isConnecting = false;
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        // Remove disconnecting player from team
        GameManager.RemovePlayer(conn.playerController.gameObject.GetComponent<Player>());
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        // Instantiate new player and assign a team
        GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
        GameManager.AddPlayer(player.GetComponent<Player>());
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
        m_connectionText = "";
        m_isConnecting = false;
    }
}
