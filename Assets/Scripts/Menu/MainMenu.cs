using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Play game button
    public void PlayGame()
    {
        SceneManager.LoadScene("NetworkMenu", LoadSceneMode.Single);
    }

    // Quit game button
    public void QuitGame()
    {
        Application.Quit();
    }
}
