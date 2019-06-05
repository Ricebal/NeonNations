using Mirror;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkLobbyManager
{
    private bool m_showStartButton;
    private bool m_isConnecting;
    private string m_connectionText;
    [SerializeField] private Canvas m_multiplayerMenu;
    [SerializeField] private Canvas m_lobbyMenu;
    [SerializeField] private Button m_buttonStart;

    private void OnEnable()
    {
        if (!Discovery.ListenForBroadcast())
        {
            m_connectionText = "An error occurred while checking online servers.";
        }
    }

    public override void Start()
    {
        base.Start();

        m_isConnecting = false;
    }

    // Display the lobby panel when a player starts or joins a server
    public override void OnStartClient()
    {
        base.OnStartClient();

        m_multiplayerMenu.gameObject.SetActive(false);
        m_lobbyMenu.gameObject.SetActive(true);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!Discovery.StartBroadcasting())
        {
            m_connectionText = "An error occurred while starting broadcasting.";
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Discovery.StopBroadcasting();
        DestroyImmediate(Discovery.Singleton.gameObject, true);
    }

    // Start a game as a host
    public void StartUpHost()
    {
        // Stop the client if he was trying to connect
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
            m_isConnecting = true;
            m_connectionText = "Connecting...";
            StartClient();
        }
    }

    // Set the IP address of the network manager for the StartClient function
    public void SetIPAddress(string ipAddress)
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

    public void StartGame()
    {
        ServerChangeScene(GameplayScene);
    }

    public void Stop()
    {
        StopHost();
        m_connectionText = "";
        m_isConnecting = false;
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
        m_connectionText = "Connection failed due to error: " + error.ToString(); ;
        m_isConnecting = false;
    }

    public override void OnLobbyServerPlayersReady()
    {
        // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null && startOnHeadless)
        {
            base.OnLobbyServerPlayersReady();
        }
        else
        {
            m_showStartButton = true;
        }
    }

    public override void OnGUI()
    {
        base.OnGUI();

        if (SceneManager.GetActiveScene().name != GameplayScene)
        {
            if (m_showStartButton && allPlayersReady)
            {
                m_buttonStart.gameObject.SetActive(true);
            }
            else
            {
                m_buttonStart.gameObject.SetActive(false);
            }
        }
    }

}