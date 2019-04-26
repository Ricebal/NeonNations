using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject Panel;
    public Text RespawnText;
    
    private float m_remainingTime;

    void Start()
    {
        Deactivate();
    }

    void Update()
    {
        if (IsActive())
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

    // Active or not the game over menu
    public void Activate(float respawnTime)
    {
        Cursor.visible = true;
        Panel.gameObject.SetActive(true);
        m_remainingTime = respawnTime;
    }

    public void Deactivate()
    {
        Cursor.visible = false;
        Panel.gameObject.SetActive(false);
    }

    // Return true if the game over menu is currently displayed, false otherwise
    public bool IsActive()
    {
        return Panel.gameObject.activeSelf;
    }

}
