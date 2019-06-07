using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private LobbyManager m_lobbyManager = null;
    [SerializeField] private Button m_buttonStartHost = null;
    // Text displayed when there is a client connection or disconnection
    [SerializeField] private TextMeshProUGUI m_connectionText = null;

    private void Update()
    {
        m_connectionText.text = m_lobbyManager.GetConnectionText();
        // The button is disabled when a client is trying to connect and vice versa
        m_buttonStartHost.interactable = !m_lobbyManager.IsConnecting();
    }

}
