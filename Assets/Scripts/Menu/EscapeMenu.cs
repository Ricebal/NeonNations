using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : NetworkBehaviour
{
    public GameObject Canvas;

    public delegate void PauseToggled();
    public event PauseToggled EventPauseToggled;

    private void Update()
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
        Canvas.SetActive(!Canvas.activeSelf);
        if (EventPauseToggled != null)
        {
            EventPauseToggled();
        }
    }

    public bool IsActive()
    {
        return Canvas.activeSelf;
    }

}
