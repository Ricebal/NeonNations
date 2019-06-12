using Mirror;
using TMPro;
using UnityEngine;

public class LobbyBotsConfig : NetworkBehaviour
{
    public static LobbyBotsConfig Singleton;

    [SerializeField] private TextMeshProUGUI m_botsText = null;
    [SerializeField] private HoldButton m_botsButtonUp = null;
    [SerializeField] private HoldButton m_botsButtonDown = null;
    [SerializeField] private GameObject m_lobbyPlayerPrefab = null;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(this);
        }
        else
        {
            Singleton = this;
        }
    }

    private void Start()
    {
        if (!isServer)
        {
            return;
        }

        m_botsButtonUp.OnValueChanged += OnBotsValueChanged;
        m_botsButtonDown.OnValueChanged += OnBotsValueChanged;
    }

    private void FixedUpdate()
    {
        // This function is periodically called to unsure that the number of bots plus the number of players 
        // does not exceed the maximum number of connections
        int amountOfBots = LobbyConfigMenu.Singleton.AmountOfBots;
        if (amountOfBots > GetMaxAmountOfBots())
        {
            LobbyConfigMenu.Singleton.AmountOfBots = GetMaxAmountOfBots();
        }
    }

    // Called when button up or button down are held down
    private void OnBotsValueChanged(HoldButton button)
    {
        int amountOfBots = LobbyConfigMenu.Singleton.AmountOfBots;
        amountOfBots += button.IncrementalValue;
        if (amountOfBots < 0 || amountOfBots > GetMaxAmountOfBots())
        {
            return;
        }

        LobbyConfigMenu.Singleton.AmountOfBots = amountOfBots;
    }

    // Return the maximum amount of bots between 0 and max connections
    private int GetMaxAmountOfBots()
    {
        return Mathf.Max(0, NetworkManager.singleton.maxConnections - NetworkManager.singleton.numPlayers);
    }

    public static void AddLobbyBot()
    {
        GameObject lobbyBot = Instantiate(Singleton.m_lobbyPlayerPrefab, GameObject.Find("Players").transform);
        lobbyBot.name = "LobbyBot";
        lobbyBot.GetComponentInChildren<TextMeshProUGUI>().text = "Bot";
        lobbyBot.transform.Find("ImageReady").gameObject.SetActive(true);

        if (Singleton.isServer)
        {
            int amountOfBots = LobbyConfigMenu.Singleton.AmountOfBots + 1;
            Singleton.m_botsText.text = amountOfBots.ToString();
        }
    }

    public static void RemoveLobbyBot()
    {
        Destroy(GameObject.Find("LobbyBot"));

        if (Singleton.isServer)
        {
            int amountOfBots = LobbyConfigMenu.Singleton.AmountOfBots - 1;
            Singleton.m_botsText.text = amountOfBots.ToString();
        }
    }
}
