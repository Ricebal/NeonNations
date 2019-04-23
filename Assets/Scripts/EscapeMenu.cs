using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class EscapeMenu : NetworkBehaviour
{
    public GameObject Canvas;
    private bool m_paused = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            TogglePause();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Disconnect()
    {
        TogglePause();
        if (isServer) 
            NetworkManager.singleton.StopHost();
        else
            NetworkManager.singleton.StopClient();
        SceneManager.LoadScene(2);
    }

    public void MainMenu()
    {
        TogglePause();
        if (isServer)
            NetworkManager.singleton.StopHost();
        else
            NetworkManager.singleton.StopClient();
        SceneManager.LoadScene(0);
    }

    public void TogglePause()
    {
        m_paused = !m_paused;
        Canvas.gameObject.SetActive(m_paused);
    }

    public bool IsPaused() {
        return m_paused;
    }
}
