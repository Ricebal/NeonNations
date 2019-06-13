using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar(hook = nameof(OnUsernameSet))] private string m_username;
    [SerializeField] private TextMeshProUGUI m_textUsername = null;
    [SerializeField] private Image m_imageReady = null;
    private Button m_buttonReady;
    private TextMeshProUGUI m_buttonReadyText;

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkLobbyManager lobby = NetworkManager.singleton as NetworkLobbyManager;

        if (lobby != null && SceneManager.GetActiveScene().name == lobby.LobbyScene)
        {
            gameObject.transform.SetParent(GameObject.Find("Players").transform);
            gameObject.transform.localScale = Vector3.one;

            m_buttonReady = GameObject.Find("ButtonReady").GetComponent<Button>();
            m_buttonReadyText = m_buttonReady.GetComponentInChildren<TextMeshProUGUI>();
            m_buttonReady.onClick.AddListener(ToggleReadyState);
        }
    }

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            CmdUsername(ProfileMenu.GetUsername());
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

    // Toggle the state of the player (ready or not) when clicking on the checkmark button
    public void ToggleReadyState()
    {
        if (NetworkClient.active && isLocalPlayer)
        {
            CmdChangeReadyState(!ReadyToBegin);
        }
    }

    public override void OnClientReady(bool readyState)
    {
        // Enable the checkmark depending on the state of the player
        m_imageReady.gameObject.SetActive(readyState);

        if (isLocalPlayer)
        {
            if (readyState)
            {
                m_buttonReadyText.text = "Not Ready";
            }
            else
            {
                m_buttonReadyText.text = "Ready";
            }
        }
    }

}
