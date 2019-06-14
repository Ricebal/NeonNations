/**
 * Authors: David, Stella, Nicander, Chiel
 */

using Mirror;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkLobbyManager
{
    public string InfoText;
    private bool m_showStartButton;

    [SerializeField] private Canvas m_multiplayerMenu = null;
    [SerializeField] private Canvas m_lobbyMenu = null;
    [SerializeField] private Canvas m_loadingScreen = null;
    [SerializeField] private Button m_buttonStart = null;

    // Display the lobby panel when a player starts or joins a server
    public override void OnStartClient()
    {
        base.OnStartClient();
        m_lobbyMenu.gameObject.SetActive(true);
        m_multiplayerMenu.gameObject.SetActive(false);
    }

    // Called when the user create a lobby
    public override void OnStartServer()
    {
        base.OnStartServer();
        Discovery.StartBroadcasting();
    }

    // Called when the user leaves the lobby and gets back to main menu
    public override void OnStopHost()
    {
        base.OnStopHost();
        Discovery.Stop();
        DestroyNetworkDiscovery();
    }

    // Set the IP address of the network manager for the StartClient function
    public void SetIPAddress(string ipAddress)
    {
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    // Called on the host when he starts the game
    public void StartGame()
    {
        Discovery.Stop();
        DestroyNetworkDiscovery();
        DisplayLoadingScreen();
        ServerChangeScene(GameplayScene);
    }

    private void DisplayLoadingScreen()
    {
        m_loadingScreen.gameObject.SetActive(true);
        m_lobbyMenu.gameObject.SetActive(false);
    }

    private void DestroyNetworkDiscovery()
    {
        GameObject networkDiscovery = GameObject.Find("NetworkDiscovery");
        if (networkDiscovery != null)
        {
            Destroy(networkDiscovery);
        }
    }

    // Called on the client when a scene is changed by the host
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
        InfoText = "Connection failed due to error: " + error.ToString();
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

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == GameplayScene)
        {
            GameManager.RemovePlayer(conn.playerController.gameObject.GetComponent<Soldier>());
        }
        base.OnServerDisconnect(conn);
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
