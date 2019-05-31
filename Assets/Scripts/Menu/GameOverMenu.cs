using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu Singleton;
    public GameObject Panel;
    public Text RespawnText;

    private float m_remainingTime;

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
        Deactivate();
    }

    private void Update()
    {
        if (Panel.activeSelf)
        {
            m_remainingTime -= Time.deltaTime;
            if (m_remainingTime < 0)
            {
                m_remainingTime = 0;
            }
            RespawnText.text = "Respawning in " + Math.Ceiling(m_remainingTime) + " seconds...";
        }
    }

    public void Respawn()
    {
        //m_remainingTime = 0;
    }

    public static void Activate(float respawnTime)
    {
        Singleton.Panel.SetActive(true);
        Singleton.m_remainingTime = respawnTime;
    }

    public static void Deactivate()
    {
        Singleton.Panel.SetActive(false);
    }

    public static bool IsActive()
    {
        return Singleton.Panel.activeSelf;
    }

}
