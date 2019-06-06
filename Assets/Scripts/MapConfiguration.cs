using UnityEngine;
using UnityEngine.Serialization;

public class MapConfiguration : MonoBehaviour
{
    public static MapConfiguration Singleton;

    [FormerlySerializedAs("m_DontDestroyOnLoad")] public bool dontDestroyOnLoad = true;


    private void Start()
    {
        InitializeSingleton();
    }

    private void InitializeSingleton()
    {
        if (Singleton != null && Singleton == this)
        {
            return;
        }

        if (dontDestroyOnLoad)
        {
            if (Singleton != null)
            {
                Debug.LogWarning("Multiple MapConfigurations detected in the scene. Only one MapConfiguration can exist at a time. The duplicate MapConfiguration will be destroyed.");
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Singleton = this;
        }
    }

    public static void DestroyMapConfig()
    {
        Destroy(Singleton.gameObject);
    }
}
