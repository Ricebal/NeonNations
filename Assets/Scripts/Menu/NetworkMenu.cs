﻿using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkMenu : MonoBehaviour
{
    [SerializeField] private LobbyManager m_lobbyManager;
    [SerializeField] private Button m_buttonStartHost;
    [SerializeField] private Button m_buttonJoinGame;
    [SerializeField] private InputField m_ipAddressField;
    
    // Text displayed when there is a client connection or disconnection
    [SerializeField] private TextMeshProUGUI m_connectionText;

    private void Update()
    {
        m_connectionText.text = m_lobbyManager.GetConnectionText();
        // The buttons are disabled when a client is trying to connect and vice versa
        m_buttonStartHost.interactable = !m_lobbyManager.IsConnecting();
        m_buttonJoinGame.interactable = !m_lobbyManager.IsConnecting();

        // Trigger join game button click when the user presses 'enter' and the ip address is specified
        if (Input.GetKey(KeyCode.Return) && m_ipAddressField.text.Length != 0)
        {
            m_lobbyManager.JoinGame();
        }
    }

}
