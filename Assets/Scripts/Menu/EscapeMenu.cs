using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : NetworkBehaviour
{
    public GameObject Canvas;

    public delegate void PauseToggledDelegate();
    public event PauseToggledDelegate OnPauseToggled;

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
        OnPauseToggled?.Invoke();
    }

    public bool IsActive()
    {
        return Canvas.activeSelf;
    }

}
