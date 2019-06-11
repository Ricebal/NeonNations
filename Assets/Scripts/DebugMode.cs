using UnityEngine;
using UnityEngine.UI;

public class DebugMode : MonoBehaviour
{
    public static DebugMode Singleton;

    [SerializeField] private Light m_directionalLight = null;
    [SerializeField] private Text m_text = null;

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
        m_text.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
#if (UNITY_EDITOR)
            {
                m_directionalLight.gameObject.SetActive(!m_directionalLight.gameObject.activeSelf);
            }
#endif
            m_text.gameObject.SetActive(!m_text.gameObject.activeSelf);
        }
    }

    public static void SetSeed(string seed)
    {
        Singleton.m_text.text = "Map seed: " + seed;
    }
}
