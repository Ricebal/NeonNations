using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class LobbyConfigMenu : NetworkBehaviour
{
    public static LobbyConfigMenu Singleton;

    public Dictionary<string, int> MapOptions;
    public int AmountOfBots = 0;

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
        }
    }

    private void Start()
    {
        // Do not display the configuration if the player is not the host
        if (!isServer)
        {
            gameObject.SetActive(false);
            return;
        }

        // Dictionary that contains the name of the option to configure associated with its value
        MapOptions = new Dictionary<string, int> { { "Map width", m_mapWidth }, { "Map height", m_mapHeight },
            { "Max room amount", m_maxRoomAmount }, { "Max shortcut amount", m_maxShortcutAmount },
            { "Min room length", m_minRoomLength }, { "Max room length", m_maxRoomLength }, {"Min tunnel length", m_minTunnelLength },
            { "Max tunnel length", m_maxTunnelLength }, { "Tunnel width", m_tunnelWidth }, {"Breakable tunnel chance", m_breakableTunnelChance },
            { "Shortcut min skip distance", m_shortcutMinSkipDistance }, {"Reflector area size", m_reflectorAreaSize } };

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
}
