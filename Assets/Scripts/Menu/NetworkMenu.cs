using TMPro;
using UnityEngine;

public class NetworkMenu : MonoBehaviour
{
    // Text displayed when there is a client connection or disconnection
    [SerializeField] private TextMeshProUGUI m_infoText = null;
    [SerializeField] private LobbyManager m_lobbyManager = null;

    private void FixedUpdate()
    {
        m_infoText.text = m_lobbyManager.InfoText;
    }

}
