using TMPro;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    public static DebugMode Singleton;

    [SerializeField] private Light m_directionalLight;
    [SerializeField] private TextMeshProUGUI m_text;

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

    private void Start()
    {
        m_directionalLight.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            m_directionalLight.gameObject.SetActive(!m_directionalLight.gameObject.activeSelf);
        }
    }

    public static void SetSeed(string seed)
    {
        Singleton.m_text.text = seed;
    }
}
