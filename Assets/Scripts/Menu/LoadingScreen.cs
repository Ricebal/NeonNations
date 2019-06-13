/**
 * Authors: David
 */

using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Singleton;

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

}
