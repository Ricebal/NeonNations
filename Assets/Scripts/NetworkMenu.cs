using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkMenu : MonoBehaviour
{
    public Button ButtonStartHost;
    public Button ButtonJoingGame;
    public Button ButtonBack;

    private NetworkManagerCustom m_networkManagerCustom;

    void Awake() {
        m_networkManagerCustom = GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>();
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnNetworkMenuLoaded;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnNetworkMenuLoaded;
    }

    private void OnNetworkMenuLoaded(Scene scene, LoadSceneMode mode) {
        // If the NetworkMenu scene is loaded...
        if (scene.name == "NetworkMenu") {
            // if the button start host is clicked, call the StartUpHost function
            ButtonStartHost.onClick.RemoveAllListeners();
            ButtonStartHost.onClick.AddListener(m_networkManagerCustom.StartUpHost);

            // if the button join game is clicked, call the JoinGame function
            ButtonJoingGame.onClick.RemoveAllListeners();
            ButtonJoingGame.onClick.AddListener(m_networkManagerCustom.JoinGame);

            // if the button start host is clicked, call the LoadMainMenu function
            ButtonBack.onClick.RemoveAllListeners();
            ButtonBack.onClick.AddListener(LoadMainMenu);
        }
    }

    // Go to the MainMenu scene
    private void LoadMainMenu() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

}
