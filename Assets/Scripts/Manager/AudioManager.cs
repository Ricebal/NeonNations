/**
 * Authors: David
 */

using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private AudioSource m_audioSource;
    [SerializeField] private float m_buttonClickVolume = 0;
    [SerializeField] private float m_menuMusicVolume = 0;
    [SerializeField] private float m_gameMusicVolume = 0;
    [SerializeField] private AudioClip m_buttonClickSound = null;
    [SerializeField] private AudioClip m_menuMusic = null;
    [SerializeField] private AudioClip m_gameMusic = null;
    [Scene][SerializeField] private List<string> m_menuScenes = null;
    [Scene][SerializeField] private List<string> m_gameScenes = null;
    private string m_lastSceneName;

    // Instantiate the AudioManager when the game starts
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        Object obj = Resources.Load("AudioManager");
        GameObject audioManager = Instantiate(obj)as GameObject;
        // Rename AudioManager(Clone) to AudioManager
        audioManager.name = obj.name;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_audioSource = GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += SceneChangedHandler;
    }

    private void Update()
    {
        // If mouse-left button is pressed and the mouse is over a button, play the button click sound
        if (Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject?.GetComponent<ButtonHovering>() != null)
        {
            m_audioSource.PlayOneShot(m_buttonClickSound, m_buttonClickVolume);
        }
    }

    private void SceneChangedHandler(Scene previousScene, Scene newScene)
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
