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

    private NetworkManagerCustom m_networkManagerCustom;

    private void Awake()
    {
        m_networkManagerCustom = GameObject.Find("NetworkManager").GetComponent<NetworkManagerCustom>();
    }

    private void Update()
    {
        ConnectionText.text = m_networkManagerCustom.GetConnectionText();
        // The buttons are disabled when a client is trying to connect and vice versa
        ButtonJoingGame.interactable = !m_networkManagerCustom.IsConnecting();
        ButtonStartHost.interactable = !m_networkManagerCustom.IsConnecting();

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
    private void LoadMainMenu()
    {
        m_networkManagerCustom.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

}
