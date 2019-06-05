using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;

public class MapConfiguration : NetworkBehaviour
{
    public static MapConfiguration Singleton;

    [FormerlySerializedAs("m_DontDestroyOnLoad")] public bool dontDestroyOnLoad = true;

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

    private void Start()
    {
        InitializeSingleton();

        m_mapOptions = new Dictionary<string, int> { { "Map width", m_mapWidth }, { "Map height", m_mapHeight }, { "Max room amount", m_maxRoomAmount }, { "Max shortcut amount", m_maxShortcutAmount },
            { "Min room length", m_minRoomLength }, { "Max room length", m_maxRoomLength }, {"Min tunnel length", m_minTunnelLength }, {"Max tunnel length", m_maxTunnelLength },
            { "Tunnel width", m_tunnelWidth }, {"Breakable tunnel chance", m_breakableTunnelChance }, {"Shortcut min skip distance", m_shortcutMinSkipDistance }, {"Reflector area size", m_reflectorAreaSize } };

        foreach(KeyValuePair<string, int> mapOption in m_mapOptions)
        {
            GameObject optionItem = Instantiate(m_optionItemPrefab, m_optionList.transform);
            optionItem.transform.localScale = Vector3.one;

            TextMeshProUGUI optionNameText = optionItem.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            optionNameText.text = mapOption.Key;

            TextMeshProUGUI optionValue = optionItem.transform.Find("Value").GetComponent<TextMeshProUGUI>();
            optionValue.text = mapOption.Value.ToString();
            optionValue.name = mapOption.Key;

            Button buttonUp = optionItem.transform.Find("ButtonUp").GetComponent<Button>();
            Button buttonDown = optionItem.transform.Find("ButtonDown").GetComponent<Button>();

            if (isServer)
            {
                buttonUp.onClick.AddListener(() => { ValueUp(optionValue); });
                buttonDown.onClick.AddListener(() => { ValueDown(optionValue); });
            }
            else
            {
                buttonUp.gameObject.SetActive(false);
                buttonDown.gameObject.SetActive(false);
            }
        }
    }

    private void InitializeSingleton()
    {
        if (Singleton != null && Singleton == this)
        {
            return;
        }

        if (dontDestroyOnLoad)
        {
            if (Singleton != null)
            {
                Debug.LogWarning("Multiple MapConfigurations detected in the scene. Only one MapConfiguration can exist at a time. The duplicate MapConfiguration will be destroyed.");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Singleton = this;
        }
    }

    public void ValueUp(TextMeshProUGUI optionName)
    {
        m_mapOptions[optionName.name] += 1;
        optionName.text = m_mapOptions[optionName.name].ToString();
    }

    public void ValueDown(TextMeshProUGUI optionName)
    {
        m_mapOptions[optionName.name] -= 1;
        if (m_mapOptions[optionName.name] < 0)
        {
            m_mapOptions[optionName.name] = 0;
        }
        optionName.text = m_mapOptions[optionName.name].ToString();
    }

    public static void DestroyMapConfig()
    {
        Destroy(Singleton.gameObject);
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

    /*public void OnPointerUp(PointerEventData eventData)
    {
        throw new System.NotImplementedException();

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }*/
}
