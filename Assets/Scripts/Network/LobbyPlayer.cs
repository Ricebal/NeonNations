using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayer : NetworkLobbyPlayer
{
    [SyncVar(hook = nameof(OnUsernameSet))] private string m_username;
    [SerializeField] private TextMeshProUGUI m_textUsername;
    [SerializeField] private Image m_imageReady;
    private Button m_buttonReady;
    private TextMeshProUGUI m_textReady;

    private new void Start()
    {
        m_buttonReady = GameObject.Find("ButtonReady").GetComponent<Button>();
        m_textReady = m_buttonReady.GetComponentInChildren<TextMeshProUGUI>();
        m_buttonReady.onClick.AddListener(ToggleReadyState);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkLobbyManager lobby = NetworkManager.singleton as NetworkLobbyManager;

        if (lobby != null && SceneManager.GetActiveScene().name == lobby.LobbyScene)
        {
            gameObject.transform.SetParent(GameObject.Find("Players").transform);
            gameObject.transform.localScale = Vector3.one;
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
            if (ReadyToBegin)
            {
                CmdChangeReadyState(false);
                m_textReady.text = "Ready";
            }
            else
            {
                CmdChangeReadyState(true);
                m_textReady.text = "Not Ready";
            }
        }
    }

    public override void OnClientReady(bool readyState)
    {
        // Enable the checkmark depending on the state of the player
        m_imageReady.gameObject.SetActive(readyState);
    }

}