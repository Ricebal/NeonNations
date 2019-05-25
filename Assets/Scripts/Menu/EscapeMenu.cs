using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : NetworkBehaviour
{
    public GameObject Canvas;

    public delegate void PauseToggledDelegate();
    public event PauseToggledDelegate OnPauseToggled;

    private bool m_paused = false;

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
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
        SceneManager.LoadScene(2);
    }

    public void TogglePause()
    {
        m_paused = !m_paused;
        Cursor.visible = m_paused;
        Canvas.gameObject.SetActive(m_paused);
        if (OnPauseToggled != null)
        {
            OnPauseToggled();
        }
    }

}
