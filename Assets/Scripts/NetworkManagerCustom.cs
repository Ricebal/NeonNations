﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManagerCustom : NetworkManager
{
    void OnEnable() {
        SceneManager.sceneLoaded += OnLobbyLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLobbyLoaded;
    }

    private void OnLobbyLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Lobby") {
            GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ButtonStartHost").GetComponent<Button>().onClick.AddListener(StartUpHost);

            GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.RemoveAllListeners();
            GameObject.Find("ButtonJoinGame").GetComponent<Button>().onClick.AddListener(JoinGame);
        }
    }

    public void StartUpHost() {
        SetPort();
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame() {
        SetIPAddress();
        SetPort();
        NetworkManager.singleton.StartClient();
    }

    void SetIPAddress() {
        string ipAddress = GameObject.Find("InputFieldIPAddress").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort() {
        NetworkManager.singleton.networkPort = 7777;
    }

}
