using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkLobbyPlayerExt : NetworkLobbyPlayer
{
    [SyncVar(hook = nameof(OnUsernameSet))] private string m_username;
    [SerializeField] private TextMeshProUGUI m_textUsername;
    [SerializeField] private Button m_buttonReady;

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkLobbyManager lobby = NetworkManager.singleton as NetworkLobbyManager;

        if (lobby != null && SceneManager.GetActiveScene().name == lobby.LobbyScene)
        {
            gameObject.transform.SetParent(GameObject.Find("Players").transform);
        }
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            CmdUsername(ProfileMenu.GetUsername());
        }
        else
        {
            m_buttonReady.interactable = false;
        }
    }

    [Command]
    private void CmdUsername(string username)
    {
        m_username = username;
    }

    private void OnUsernameSet(string username)
    {
        m_textUsername.text = username;
    }

    // Change the state of the player (ready or not) when clicking on the checkmark button
    public void ChangeReadyState()
    {
        if (NetworkClient.active && isLocalPlayer)
        {
            if (ReadyToBegin)
            {
                CmdChangeReadyState(false);
            }
            else
            {
                CmdChangeReadyState(true);
            }
        }
    }

    public override void OnClientReady(bool readyState)
    {
        // Change the color of the checkmark button depending on the state of the player
        Image image = m_buttonReady.GetComponent<Image>();
        if (readyState)
        {
            image.color = new Color32(83, 255, 40, 255);
        }
        else
        {
            image.color = new Color32(255, 255, 255, 255);
        }
    }

}