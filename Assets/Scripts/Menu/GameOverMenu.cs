using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject Panel;
    public Text RespawnText;
    public Button RespawnButton;

    public delegate void RespawnClickDelegate();
    public event RespawnClickDelegate OnRespawnClick;

    private float m_remainingTime;

    private void Start()
    {
        Deactivate();
    }

    private void Update()
    {
        if (Panel.activeSelf)
        {
            m_remainingTime -= Time.deltaTime;
            if (m_remainingTime <= 0)
            {
                RespawnText.text = "You can respawn!";
                RespawnButton.interactable = true;
            }
            else
            {
                RespawnText.text = "Respawning in " + Math.Ceiling(m_remainingTime) + " seconds...";
                RespawnButton.interactable = false;
            }
        }
    }

    public void Respawn()
    {
        OnRespawnClick?.Invoke();
    }

    public void Activate(float respawnTime)
    {
        Panel.SetActive(true);
        m_remainingTime = respawnTime;
    }

    public void Deactivate()
    {
        Panel.SetActive(false);
    }

    public bool IsActive()
    {
        return Panel.activeSelf;
    }

}
