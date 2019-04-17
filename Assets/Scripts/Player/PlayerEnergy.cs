using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerEnergy : NetworkBehaviour
{
    public Slider EnergySlider;

    private int m_startingEnergy;
    private int m_currentEnergy;

    void Start() {
        EnergySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
        m_startingEnergy = (int) EnergySlider.value;
        m_currentEnergy = m_startingEnergy;
    }

    void Update() {
        if (!isLocalPlayer) {
            return;
        }
        EnergySlider.value = m_currentEnergy;
    }

    public void AddEnergy(int value) {
        m_currentEnergy += value;
        if (m_currentEnergy > 100) { 
            m_currentEnergy = 100;  
        }
        if (m_currentEnergy < 0) {
            m_currentEnergy = 0;
        }
    }

    public int GetCurrentEnergy() {
        return m_currentEnergy;
    }
}
