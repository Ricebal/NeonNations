using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerCustom : NetworkManager
{

    private bool m_isConnecting;
    private bool m_thereIsConnection;

    void Start() {
        m_isConnecting = false;
    }

    // Start a game as a host
    public void StartUpHost() {
        if (m_isConnecting) {
            NetworkManager.singleton.StopClient();
        }
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    // Join a game as a client
    public void JoinGame() {
        if (!m_isConnecting) {
            m_isConnecting = true;
            SetIPAddress();
            TestConnection();
            if (m_thereIsConnection) {
                SetPort();
                NetworkManager.singleton.StartClient();
            }
            else {
                Debug.Log("no connection");
            }
        }
    }

    // Set the IP address of the network manager for the StartClient function
    void SetIPAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.Find("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    // Set the port of the network manager for the StartClient function
    void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

    IEnumerator TestConnection() {
        float timeTaken = 0;
        float maxTime = 2;

        while (true) {
            Ping testPing = new Ping(NetworkManager.singleton.networkAddress);
            timeTaken = 0;
            while (!testPing.isDone) {
                timeTaken += Time.deltaTime;
                if (timeTaken > maxTime) {
                    m_thereIsConnection = false;
                    break;
                }

                yield return null;
            }

            if (timeTaken <= maxTime) {
                m_thereIsConnection = true;
            }

            yield return null;
        }
    }

}
