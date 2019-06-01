using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private AudioSource m_audioSource;
    [SerializeField] private float m_menuMusicVolume;
    [SerializeField] private float m_gameMusicVolume;
    [SerializeField] private AudioClip m_menuMusic;
    [SerializeField] private AudioClip m_gameMusic;
    [Scene] [SerializeField] private List<string> m_menuScenes;
    [Scene] [SerializeField] private List<string> m_gameScenes;
    private string m_lastSceneName;

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
        m_audioSource = GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene previousScene, Scene newScene)
    {
        // If the new scene is a menu scene and the previous scene was a game scene or the game just started, play menu music
        if (m_menuScenes.Contains(newScene.name) && m_gameScenes.Contains(m_lastSceneName) || m_lastSceneName == null)
        {
            Play(m_menuMusic, m_menuMusicVolume);
        }
        // If the new scene is a game scene and the previous scene was a menu scene, play game music
        else if (m_gameScenes.Contains(newScene.name) && m_menuScenes.Contains(m_lastSceneName))
        {
            Play(m_gameMusic, m_gameMusicVolume);
        }
        // previousScene always returns an empty scene with no name, we need to store the last scene on our own
        m_lastSceneName = newScene.name;
    }

    // Play in loop the clip with the provided volume
    private void Play(AudioClip clip, float volume)
    {
        m_audioSource.Stop();
        m_audioSource.clip = clip;
        m_audioSource.volume = volume;
        m_audioSource.Play();
    }
}
