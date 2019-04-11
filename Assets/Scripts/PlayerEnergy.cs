using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEnergy : MonoBehaviour
{
    public int StartingEnergy = 100;
    public int CurrentEnergy;
    public Slider EnergySlider;

    // Start is called before the first frame update
    void Start()
    {
        CurrentEnergy = StartingEnergy;
        EnergySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        EnergySlider.value = CurrentEnergy;
    }

    public void AddEnergy(int value) {
        CurrentEnergy += value;
        if(CurrentEnergy > 100)
            CurrentEnergy = 100;   
        if(CurrentEnergy < 0)
            CurrentEnergy = 0;
    }
}
