using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LobbyConfig : NetworkBehaviour
{
    public static LobbyConfig Singleton;

    public Dictionary<string, int> MapOptions;
    [SyncVar(hook = nameof(OnAmountOfBots))] public int AmountOfBots = 0;

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
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (isServer)
        {
            return;
        }

        // Dictionary that contains the name of the option to configure associated with its value
        MapOptions = new Dictionary<string, int> {
            { "Map width", m_mapWidth },
            { "Map height", m_mapHeight },
            { "Max room amount", m_maxRoomAmount },
            { "Max shortcut amount", m_maxShortcutAmount },
            { "Min room length", m_minRoomLength },
            { "Max room length", m_maxRoomLength },
            { "Min tunnel length", m_minTunnelLength },
            { "Max tunnel length", m_maxTunnelLength },
            { "Tunnel width", m_tunnelWidth },
            { "Breakable tunnel chance", m_breakableTunnelChance },
            { "Shortcut min skip distance", m_shortcutMinSkipDistance },
            { "Reflector area size", m_reflectorAreaSize } };
    }

    public static int GetOptionValue(string optionName)
    {
        if (Singleton.MapOptions.ContainsKey(optionName) == false)
        {
            Debug.LogError("MapConfiguration::GetOptionValue - No action named: " + optionName);
            return -1;
        }
        return Singleton.MapOptions[optionName];
    }

    public static int GetAmountOfBots()
    {
        return Singleton.AmountOfBots;
    }

    // Called when the amount of bots is changed, used to synchronize lobby bots with all clients
    private void OnAmountOfBots(int newAmountOfBots)
    {
        // Number of bots added / removed
        int deltaBots = newAmountOfBots - AmountOfBots;
        for (int i = 0; i < Mathf.Abs(deltaBots); i++)
        {
            if (deltaBots < 0)
            {
                LobbyBotsConfig.RemoveLobbyBot();
            }
            else
            {
                LobbyBotsConfig.AddLobbyBot();
            }
        }
    }

}