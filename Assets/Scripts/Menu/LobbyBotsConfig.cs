using Mirror;
using TMPro;
using UnityEngine;

public class LobbyBotsConfig : NetworkBehaviour
{
    [SerializeField] private HoldButton m_botsButtonUp = null;
    [SerializeField] private HoldButton m_botsButtonDown = null;
    [SerializeField] private GameObject m_lobbyPlayerPrefab = null;

    private void Start()
    {
        if (!isServer)
        {
            return;
        }

        m_botsButtonUp.OnValueChanged += OnBotsValueChanged;
        m_botsButtonDown.OnValueChanged += OnBotsValueChanged;
    }

    private void OnBotsValueChanged(HoldButton button)
    {
        int maxBots = NetworkManager.singleton.maxConnections - NetworkManager.singleton.numPlayers;

        int amountOfBots = LobbyConfigMenu.Singleton.AmountOfBots;
        amountOfBots += button.IncrementalValue;
        if (amountOfBots < 0 || amountOfBots > maxBots)
        {
            return;
        }

        if (button.IncrementalValue == 1)
        {
            RpcAddLobbyBot(amountOfBots);
        }
        else
        {
            RpcRemoveLobbyBot(amountOfBots + 1);
        }

        LobbyConfigMenu.Singleton.AmountOfBots = amountOfBots;
        button.transform.parent.Find("Value").GetComponent<TextMeshProUGUI>().text = amountOfBots.ToString();
    }

    [ClientRpc]
    private void RpcAddLobbyBot(int id)
    {
        GameObject lobbyBot = Instantiate(m_lobbyPlayerPrefab, GameObject.Find("Players").transform);
        lobbyBot.name = "Bot " + id;
        lobbyBot.GetComponentInChildren<TextMeshProUGUI>().text = "Bot";
        lobbyBot.transform.Find("ImageReady").gameObject.SetActive(true);
    }

    [ClientRpc]
    private void RpcRemoveLobbyBot(int id)
    {
        Destroy(GameObject.Find("Bot " + id));
    }
}
