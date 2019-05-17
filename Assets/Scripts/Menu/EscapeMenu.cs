using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class EscapeMenu : NetworkBehaviour
{
    public GameObject Canvas;
    public delegate void PauseToggled();
    public event PauseToggled EventPauseToggled;

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
        if (EventPauseToggled != null)
        {
            EventPauseToggled();
        }
    }

}
