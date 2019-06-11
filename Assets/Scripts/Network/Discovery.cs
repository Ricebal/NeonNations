using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Discovery : NetworkDiscovery
{
    public static Discovery Singleton;

    private LobbyManager m_lobbyManager;
    private GameObject m_serverObj;
    private GameObject m_serversObj;
    private Dictionary<string, Server> m_servers = new Dictionary<string, Server>();

    private class Server
    {
        public GameObject Prefab;
        public string Ip;
        public string Data;
        public float LastPing;

        public Server(GameObject prefab, string ip, string data, float lastPing)
        {
            Prefab = prefab;
            Ip = ip;
            Data = data;
            LastPing = lastPing;
        }
    }

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
        m_lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        m_serverObj = Resources.Load("Server") as GameObject;
        m_serversObj = GameObject.Find("Servers");

        Initialize();
        StartListening();
    }

    public void FixedUpdate()
    {
        /*
        Servers emit a data at a broadcast interval when they are online and stop emitting when they are offline.
        Servers that did not send data for more than broadcast interval milliseconds are offlines and need to be removed from the dictionary.
        ToList() is used to avoid concurrent modification error (removing element while iterating).
       */
        foreach (Server server in m_servers.Values.ToList())
        {
            // The broadcast interval is multiplied by a factor to add a little margin, avoiding false timeout detection
            if ((Time.time - server.LastPing) * 1000 > broadcastInterval * 5f)
            {
                Destroy(server.Prefab);
                m_servers.Remove(server.Ip);
            }
        }
    }

    public static void StartListening()
    {
        Stop();
        // An error can be shown if the broadcasting port is already used,
        // probably due to multiple instances of the game running simultaneously
        if (!Singleton.StartAsClient())
        {
            Singleton.m_lobbyManager.InfoText = "An error occurred while checking online servers.";
        }
    }

    public static void StartBroadcasting()
    {
        Stop();
        Singleton.broadcastData = ProfileMenu.GetUsername();
        if (!Singleton.StartAsServer())
        {
            Singleton.m_lobbyManager.InfoText = "An error occurred while starting broadcasting.";
        }
    }

    public static void Stop()
    {
        if (Singleton.running)
        {
            Singleton.StopBroadcast();
        }
    }

    public override void OnReceivedBroadcast(string serverIp, string data)
    {
        if (m_servers.ContainsKey(serverIp))
        {
            m_servers[serverIp].LastPing = Time.time;
        }
        else
        {
            GameObject serverPrefab = Instantiate(m_serverObj, m_serversObj.transform);
            serverPrefab.GetComponentInChildren<Button>().onClick.AddListener(() => Join(serverIp));
            serverPrefab.GetComponentInChildren<TextMeshProUGUI>().text = "[" + serverIp + "] " + data;

            m_servers.Add(serverIp, new Server(serverPrefab, serverIp, data, Time.time));
        }
    }

    private void Join(string serverIp)
    {
        Discovery.Stop();
        m_lobbyManager.SetIPAddress(serverIp);
        m_lobbyManager.StartClient();
    }

}
