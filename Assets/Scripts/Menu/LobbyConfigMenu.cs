using Mirror;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyConfigMenu : NetworkBehaviour
{
    public static LobbyConfigMenu Singleton;

    [SerializeField] private GameObject m_optionItemPrefab = null;
    [SerializeField] private GameObject m_mapOptionList = null;
    [SerializeField] private HoldButton m_botsButtonUp = null;
    [SerializeField] private HoldButton m_botsButtonDown = null;

    private Dictionary<string, int> m_mapOptions;

    private int m_mapWidth = 50;
    private int m_mapHeight = 50;
    private int m_maxRoomAmount = 100;
    private int m_maxShortcutAmount = 10;
    private int m_minRoomLength = 6;
    private int m_maxRoomLength = 9;
    private int m_minTunnelLength = 1;
    private int m_maxTunnelLength = 7;
    private int m_tunnelWidth = 2;
    private int m_breakableTunnelChance = 20;
    private int m_shortcutMinSkipDistance = 20;
    private int m_reflectorAreaSize = 200;
    private int m_amountOfBots = 0;

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
        // Do not display the configuration if the player is not the host
        if (!isServer)
        {
            return;
        }

        // Active configuration panel for the Host
        gameObject.SetActive(true);

        // Dictionary that contains the name of the option to configure associated with its value
        m_mapOptions = new Dictionary<string, int> { { "Map width", m_mapWidth }, { "Map height", m_mapHeight },
            { "Max room amount", m_maxRoomAmount }, { "Max shortcut amount", m_maxShortcutAmount },
            { "Min room length", m_minRoomLength }, { "Max room length", m_maxRoomLength }, {"Min tunnel length", m_minTunnelLength },
            { "Max tunnel length", m_maxTunnelLength }, { "Tunnel width", m_tunnelWidth }, {"Breakable tunnel chance", m_breakableTunnelChance },
            { "Shortcut min skip distance", m_shortcutMinSkipDistance }, {"Reflector area size", m_reflectorAreaSize } };

        foreach (KeyValuePair<string, int> mapOption in m_mapOptions)
        {
            // Create one "Option Item" per element defined in the mapOptions dictionary
            GameObject optionItem = Instantiate(m_optionItemPrefab, m_mapOptionList.transform);
            optionItem.transform.localScale = Vector3.one;
            optionItem.name = mapOption.Key;

            // Set the name of the option in the "Option List"
            TextMeshProUGUI optionNameText = optionItem.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            optionNameText.text = mapOption.Key;

            // Set the default value of the option in the "Option List"
            TextMeshProUGUI optionValue = optionItem.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            optionValue.text = mapOption.Value.ToString();

            // Add the possibility to increase the value of the option
            HoldButton buttonUp = optionItem.transform.Find("ButtonUp").GetComponent<HoldButton>();
            buttonUp.OnValueChanged += OnValueChanged;

            // Add the possibility to decrease the value of the option
            HoldButton buttonDown = optionItem.transform.Find("ButtonDown").GetComponent<HoldButton>();
            buttonDown.OnValueChanged += OnValueChanged;
        }

        m_botsButtonUp.OnValueChanged += OnBotsValueChanged;
        m_botsButtonDown.OnValueChanged += OnBotsValueChanged;
    }

    // Change the value of the option depending of the incremental value of the button
    private void OnValueChanged(HoldButton button)
    {
        Transform parent = button.gameObject.transform.parent;
        m_mapOptions[parent.name] += button.IncrementalValue;
        if (m_mapOptions[parent.name] < 0)
        {
            m_mapOptions[parent.name] = 0;
        }
        else
        {
            parent.Find("Value").GetComponent<TextMeshProUGUI>().text = m_mapOptions[parent.name].ToString();
        }
    }

    private void OnBotsValueChanged(HoldButton button)
    {
        m_amountOfBots += button.IncrementalValue;
        m_amountOfBots = Mathf.Max(0, Mathf.Min(m_amountOfBots, NetworkManager.singleton.maxConnections));
        button.transform.parent.Find("Value").GetComponent<TextMeshProUGUI>().text = m_amountOfBots.ToString();
    }

    public static int GetOptionValue(string optionName)
    {
        if (Singleton.m_mapOptions.ContainsKey(optionName) == false)
        {
            Debug.LogError("MapConfiguration::GetOptionValue - No action named: " + optionName);
            return -1;
        }
        return Singleton.m_mapOptions[optionName];
    }

    public static int GetAmountOfBots()
    {
        return Singleton.m_amountOfBots;
    }
}
