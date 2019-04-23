using UnityEngine;
using UnityEngine.UI;
using System;

public class GameOverMenu : MonoBehaviour
{

    public float RespawnDelay;
    public GameObject Panel;
    public Button RespawnButton;
    public Text RespawnText;

    private float m_remainingTime;

    void Start() {
        RespawnButton.onClick.AddListener(Respawn);
        SetActive(false);
    }

    void Update() {
        if (IsActive()) {
            m_remainingTime -= Time.deltaTime;
            if (m_remainingTime < 0) {
                m_remainingTime = 0;
            }
            RespawnText.text = "Respawning in " + Math.Ceiling(m_remainingTime) + " seconds...";
        }
    }

    public void Respawn() {
        m_remainingTime = 0;
    }

    // Return the remaining time in seconds before the player's respawn
    public float GetRemainingTime() {
        return m_remainingTime;
    }

    // Active or not the game over menu
    public void SetActive(bool active) {
        Panel.gameObject.SetActive(active);
        // Reset the timer when the game over menu is disabled
        if (!active) {
            m_remainingTime = RespawnDelay;
        }
    }

    // Return true if the game over menu is currently displayed, false otherwise
    public bool IsActive() {
        return Panel.gameObject.activeSelf;
    }

}
