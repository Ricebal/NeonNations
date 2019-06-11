using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Singleton;

    [SerializeField] private TextMeshProUGUI m_loadingText = null;
    [SerializeField] private float m_interval = 0;
    private string m_initialText;
    private float m_startTime;

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
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        m_initialText = m_loadingText.text;
        m_startTime = Time.time;
    }

    private void FixedUpdate()
    {
        int dotCount = (int) ((Time.time - m_startTime) / m_interval) % 4;

        string dots = "";
        for (int i = 0; i < dotCount; i++)
        {
            dots += ".";
        }
        m_loadingText.SetText(m_initialText + dots);
    }

}
