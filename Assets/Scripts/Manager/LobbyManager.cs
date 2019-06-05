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
    [SerializeField] private Canvas m_mapConfiguration;
    [SerializeField] private Button m_buttonStart;

    public override void Start()
    {
        base.Start();

        m_isConnecting = false;
        m_connectionText = "";
    }

    // Display the lobby panel when a player starts or joins a server
    public override void OnStartClient()
    {
        base.OnStartClient();

        m_multiplayerMenu.gameObject.SetActive(false);
        m_lobbyMenu.gameObject.SetActive(true);
        m_mapConfiguration.gameObject.SetActive(true);
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
            string ipAddress = GameObject.Find("InputFieldIPAddress")?.transform.Find("Text")?.GetComponent<Text>().text;
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