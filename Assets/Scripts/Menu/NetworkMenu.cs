using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkMenu : MonoBehaviour
{
    public Button ButtonStartHost;
    public Button ButtonJoingGame;
    public Button ButtonBack;
    public InputField IpAddressField;
    // Text displayed when there is a client connection or disconnection
    public TextMeshProUGUI ConnectionText;

    private void Update()
    {
        // Trigger join game button click when the user presses 'enter' and the ip address is specified
        if (Input.GetKey(KeyCode.Return) && IpAddressField.text.Length != 0)
        {
            ButtonJoingGame.onClick.Invoke();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnNetworkMenuLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnNetworkMenuLoaded;
    }

    private void OnNetworkMenuLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the NetworkMenu scene is loaded...
        if (scene.name == "NetworkMenu")
        {
            // if the button start host is clicked, call the LoadMainMenu function
            ButtonBack.onClick.RemoveAllListeners();
            ButtonBack.onClick.AddListener(LoadMainMenu);
        }
    }

    // Go to the MainMenu scene
    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

}
