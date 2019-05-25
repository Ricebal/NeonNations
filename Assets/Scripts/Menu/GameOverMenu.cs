using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject Panel;
    public Text RespawnText;

    public delegate void RespawnClickDelegate();
    public event RespawnClickDelegate OnRespawnClick;

    private float m_remainingTime;

    void Start()
    {
        Deactivate();
    }

    void Update()
    {
        if (Panel.gameObject.activeSelf)
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
        if(OnRespawnClick != null)
        {
            OnRespawnClick();
        }
    }

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

}
