using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class EscapeMenu : NetworkBehaviour
{
    public GameObject Canvas;
    public Button ResumeButton;
    public Button DisconnectButton;
    public Button ExitButton;
    public NetworkManager NetworkManager;
    public bool Paused = false;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.gameObject.SetActive(false);
        ResumeButton.onClick.AddListener(TogglePause);
        DisconnectButton.onClick.AddListener(TogglePause);
        if(isServer)
            DisconnectButton.onClick.AddListener(NetworkManager.singleton.StopHost);
        else
            DisconnectButton.onClick.AddListener(NetworkManager.singleton.StopClient);
        ExitButton.onClick.AddListener(Application.Quit);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("escape")) {
            TogglePause();    
        }
    }

    void TogglePause() {
        Paused = !Paused;
        Canvas.gameObject.SetActive(Paused);
    }
}
