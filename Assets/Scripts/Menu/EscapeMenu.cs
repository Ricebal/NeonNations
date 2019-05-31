using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeMenu : NetworkBehaviour
{
    public static EscapeMenu Singleton;
    public GameObject Canvas;

    public delegate void PauseToggledDelegate();
    public event PauseToggledDelegate OnPauseToggled;

    private void Awake()
    {
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

    public static bool IsActive()
    {
        return Singleton.Canvas.activeSelf;
    }

}
