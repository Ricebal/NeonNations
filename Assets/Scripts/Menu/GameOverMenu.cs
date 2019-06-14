/**
 * Authors: David, Stella, Benji, Nicander
 */

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public static GameOverMenu Singleton;
    [SerializeField] private GameObject m_panel = null;
    [SerializeField] private TextMeshProUGUI m_respawnText = null;
    [SerializeField] private Button m_respawnButton = null;

    public delegate void RespawnClickDelegate();
    public event RespawnClickDelegate OnRespawnClick;

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
        if (m_panel.activeSelf)
        {
            m_remainingTime -= Time.deltaTime;
            if (m_remainingTime <= 0)
            {
                m_respawnText.text = "Respawn";
                m_respawnButton.interactable = true;
            }
            else
            {
                m_respawnText.text = Math.Ceiling(m_remainingTime).ToString();
                m_respawnButton.interactable = false;
            }
        }
    }

    public void Respawn()
    {
        OnRespawnClick?.Invoke();
    }

    public static void Activate(float respawnTime)
    {
        Singleton.m_panel.SetActive(true);
        Singleton.m_remainingTime = respawnTime;
    }

    public static void Deactivate()
    {
        Singleton.m_respawnText.text = "";
        Singleton.m_panel.SetActive(false);
    }

    public static bool IsActive()
    {
        return Singleton.m_panel.activeSelf;
    }

}
