using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using TMPro;

public class MapConfiguration : NetworkBehaviour
{
    public static MapConfiguration Singleton;

    // configuration
    [FormerlySerializedAs("m_DontDestroyOnLoad")] public bool dontDestroyOnLoad = true;

    [SerializeField] private GameObject m_configItem;

    private Dictionary<int, TextMeshProUGUI> m_mapOptions;

    [SyncVar] public int MapWidth;
    [SyncVar] public int MapHeight;
    [SyncVar] public int MaxRoomAmount;
    //public int TunnelWitdh;

    [SerializeField] private TextMeshProUGUI m_MapWidth;
    [SerializeField] private TextMeshProUGUI m_mapHeight;
    [SerializeField] private TextMeshProUGUI m_maxRoomAmount;
    //[SyncVar] [SerializeField] private TextMeshProUGUI m_tunnelWidth;

    [SerializeField] private Button m_buttonUpMapWidth;
    [SerializeField] private Button m_buttonDownMapWidth;

    private void Awake()
    {
        InitializeSingleton();
        InitializeMapOptions();
        MapWidth = 50;
        MapHeight = 50;
        MaxRoomAmount = 100;
        //TunnelWitdh = 2;
        m_MapWidth.text = MapWidth.ToString();
        m_mapHeight.text = MapHeight.ToString();
        m_maxRoomAmount.text = MaxRoomAmount.ToString();
        //m_tunnelWidth.text = TunnelWitdh.ToString();
        m_buttonUpMapWidth.onClick.AddListener(() => { ValueUp(ref MapWidth, ref m_MapWidth); });
        m_buttonDownMapWidth.onClick.AddListener(() => { ValueDown(ref MapWidth, ref m_MapWidth); });
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
        //InitializeVariables();
    }

    private void InitializeMapOptions()
    {

    }

    public void ValueUp(ref int value, ref TextMeshProUGUI valueText)
    {
        value += 1;
        valueText.text = value.ToString();
    }

    public void ValueDown(ref int value, ref TextMeshProUGUI valueText)
    {
        value -= 1;
        valueText.text = value.ToString();
    }

    public static void DestroyMapConfig()
    {
        Destroy(Singleton.gameObject);
    }

}
