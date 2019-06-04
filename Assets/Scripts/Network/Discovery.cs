using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Discovery : NetworkDiscovery
{
    public static Discovery Singleton;

    [SerializeField] private GameObject m_serverObj;
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
        // TODO: For an unknown reason, this variable does not appear in Unity editor
        m_serverObj = Resources.Load("Server") as GameObject;
        m_serversObj = GameObject.Find("Servers");
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
    
    public void FixedUpdate()
    {
        foreach(Server server in m_servers.Values)
        {
            if(Time.time - server.LastPing > broadcastInterval)
            {
                print("removing:" + server.Ip);
                Destroy(server.Prefab);
                m_servers.Remove(server.Ip);
            }
        }
    }

    public static void ListenForBroadcast()
    {
        StopBroadcasting();
        Singleton.Initialize();
        // An error can be shown if the broadcasting port is already used, this cannot be catched and can be ignored.
        // This is probably due to multiple instances of the game running simultaneously.
        Singleton.StartAsClient();
    }

    public static void StartBroadcasting()
    {
        StopBroadcasting();
        Singleton.Initialize();
        Singleton.StartAsServer();
    }

    public static void StopBroadcasting()
    {
        if (Singleton.running)
        {
            Singleton.StopBroadcast();
        }
    }

    public override void OnReceivedBroadcast(string serverIp, string data)
    {
        print("broadcast");
        if (m_servers.ContainsKey(serverIp))
        {
            m_servers[serverIp].LastPing = Time.time;
            print("updating:" + serverIp);
        }
        else
        {
            GameObject serverPrefab = Instantiate(m_serverObj, m_serversObj.transform);
            serverPrefab.GetComponentInChildren<TextMeshProUGUI>().text = data + " (IP: " + serverIp + ")";

            m_servers.Add(serverIp, new Server(serverPrefab, serverIp, data, Time.time));
            print("adding:" + serverIp);
        }
    }

}
