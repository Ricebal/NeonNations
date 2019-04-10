using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider HealthSlider;

    private int m_startingHealth = 100;
    private int m_currentHealth;
    private PlayerController m_playerController;    // Reference to the player's movement
    //private PlayerShooting m_playerShooting;
    private bool m_isDead;                            // Whether the player is dead
    private bool m_damaged;                           // True when the player gets damaged

    void Start()
    {
        m_playerController = GetComponent<PlayerController>();
        HealthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_currentHealth = m_startingHealth;
        m_isDead = false;
        m_damaged = false;
    }

    void Update()
    {
        // If the player has just been damaged, he gets slow down
        if (m_damaged)
        {

        }
        if (Input.GetKeyDown("space"))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown("b"))
        {
            TakeDamage(-10);
        }
    }

    public void TakeDamage(int amount)
    {
        m_damaged = true;
        m_currentHealth -= amount;

        HealthSlider.value = m_currentHealth;
    }
}
