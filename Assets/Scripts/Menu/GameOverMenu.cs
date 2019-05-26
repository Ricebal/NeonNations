using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject Panel;
    public Text RespawnText;

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
