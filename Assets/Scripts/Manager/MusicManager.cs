using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_menuAudioSource;
    [SerializeField] private AudioSource m_gameAudioSource;
    [Scene] [SerializeField] private List<string> m_menuScenes;
    [Scene] [SerializeField] private List<string> m_gameScenes;

    //TODO: Singleton initialized in GameManager
    // Instantiate the MusicManager when the game starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        Object obj = Resources.Load("MusicManager");
        GameObject musicManager = Instantiate(obj) as GameObject;
        // Rename MusicManager(Clone) to MusicManager
        musicManager.name = obj.name;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        if (m_menuScenes.Contains(newScene.name) && !m_menuAudioSource.isPlaying)
        {
            m_menuAudioSource.Play();
            m_gameAudioSource.Stop();
        }
        else if (m_gameScenes.Contains(newScene.name) && !m_gameAudioSource.isPlaying)
        {
            m_menuAudioSource.Stop();
            m_gameAudioSource.Play();
        }
    }
}
