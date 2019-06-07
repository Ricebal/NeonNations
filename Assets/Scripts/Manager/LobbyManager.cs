using Mirror;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkLobbyManager
{
    private bool m_showStartButton;
    private string m_infoText;
    [SerializeField] private Canvas m_multiplayerMenu = null;
    [SerializeField] private Canvas m_lobbyMenu = null;
    [SerializeField] private Canvas m_loadingScreen = null;
    [SerializeField] private Button m_buttonStart = null;

    private void OnEnable()
    {
        if (!Discovery.ListenForBroadcast())
        {
            m_infoText = "An error occurred while checking online servers.";
        }
    }

    // Display the lobby panel when a player starts or joins a server
    public override void OnStartClient()
    {
        base.OnStartClient();
        m_lobbyMenu.gameObject.SetActive(true);
        m_multiplayerMenu.gameObject.SetActive(false);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!Discovery.StartBroadcasting())
        {
            m_infoText = "An error occurred while starting broadcasting.";
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Discovery.StopBroadcasting();

        GameObject networkDiscovery = GameObject.Find("NetworkDiscovery");
        if (networkDiscovery != null)
        {
            DestroyImmediate(networkDiscovery, true);
        }
    }

    // Set the IP address of the network manager for the StartClient function
    public void SetIPAddress(string ipAddress)
    {
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    public string GetInfoText()
    {
        return m_infoText;
    }

    public void StartGame()
    {
        DisplayLoadingScreen();
        ServerChangeScene(GameplayScene);
    }

    private void DisplayLoadingScreen()
    {
        m_loadingScreen.gameObject.SetActive(true);
        m_lobbyMenu.gameObject.SetActive(false);
    }

    public override void OnClientChangeScene(string newSceneName)
    {
        base.OnClientChangeScene(newSceneName);
        if (newSceneName == GameplayScene)
        {
            DisplayLoadingScreen();
        }
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        UnityEngine.Networking.NetworkError error = (UnityEngine.Networking.NetworkError)errorCode;
        m_infoText = "Connection failed due to error: " + error.ToString(); ;
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