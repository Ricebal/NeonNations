﻿using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyConfigMenu : NetworkBehaviour
{
    public static LobbyConfigMenu Singleton;

    [SerializeField] private GameObject m_optionItemPrefab;
    [SerializeField] private GameObject m_optionList;

    private Dictionary<string, int> m_mapOptions;

    [SyncVar] private int m_mapWidth = 50;
    [SyncVar] private int m_mapHeight = 50;
    [SyncVar] private int m_maxRoomAmount = 100;
    [SyncVar] private int m_maxShortcutAmount = 10;
    [SyncVar] private int m_minRoomLength = 6;
    [SyncVar] private int m_maxRoomLength = 9;
    [SyncVar] private int m_minTunnelLength = 1;
    [SyncVar] private int m_maxTunnelLength = 7;
    [SyncVar] private int m_tunnelWidth = 2;
    [SyncVar] private int m_breakableTunnelChance = 20;
    [SyncVar] private int m_shortcutMinSkipDistance = 20;
    [SyncVar] private int m_reflectorAreaSize = 200;

    private Vector3 m_anchoredPosition;
    private Vector3 m_localScale;

    private void Awake()
    {
        m_anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
        m_localScale = GetComponent<RectTransform>().localScale;
    }

    public override void OnStartClient()
    {
        GetComponent<RectTransform>().anchoredPosition = m_anchoredPosition;
        GetComponent<RectTransform>().localScale = m_localScale;
        m_optionList.transform.localScale = Vector3.one;
    }

    private void Start()
    {
        InitializeSingleton();

        m_mapOptions = new Dictionary<string, int> { { "Map width", m_mapWidth }, { "Map height", m_mapHeight }, { "Max room amount", m_maxRoomAmount }, { "Max shortcut amount", m_maxShortcutAmount },
            { "Min room length", m_minRoomLength }, { "Max room length", m_maxRoomLength }, {"Min tunnel length", m_minTunnelLength }, {"Max tunnel length", m_maxTunnelLength },
            { "Tunnel width", m_tunnelWidth }, {"Breakable tunnel chance", m_breakableTunnelChance }, {"Shortcut min skip distance", m_shortcutMinSkipDistance }, {"Reflector area size", m_reflectorAreaSize } };

        foreach (KeyValuePair<string, int> mapOption in m_mapOptions)
        {
            GameObject optionItem = Instantiate(m_optionItemPrefab);
            optionItem.transform.SetParent(m_optionList.transform);
            optionItem.transform.localScale = Vector3.one;
            optionItem.name = mapOption.Key;

            TextMeshProUGUI optionNameText = optionItem.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            optionNameText.text = mapOption.Key;

            TextMeshProUGUI optionValue = optionItem.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            optionValue.text = mapOption.Value.ToString();

            Button buttonUp = optionItem.transform.Find("ButtonUp").GetComponent<Button>();
            Button buttonDown = optionItem.transform.Find("ButtonDown").GetComponent<Button>();

        }  

        if (isServer)
        {
            GameObject.Find("Map width/ButtonUp").GetComponent<Button>().onClick.AddListener(() => { ValueUp(ref m_mapWidth, "Map width"); });
            GameObject.Find("Map width/ButtonDown").GetComponent<Button>().onClick.AddListener(() => { ValueDown(ref m_mapWidth, "Map width"); });
        }
        else
        {
            GameObject.Find("Map width/ButtonUp").GetComponent<Button>().gameObject.SetActive(false);
            GameObject.Find("Map width/ButtonDown").GetComponent<Button>().gameObject.SetActive(false);
        }
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

    public void ValueUp(ref int value, string name)
    {
        value += 1;
        GameObject.Find(name + "/Value").GetComponent<TextMeshProUGUI>().text = value.ToString();
    }

    public void ValueDown(ref int value, string name)
    {
        value -= 1;
        if (value < 0)
        {
            value = 0;
        }
        GameObject.Find(name + "/Value").GetComponent<TextMeshProUGUI>().text = value.ToString();
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
}
