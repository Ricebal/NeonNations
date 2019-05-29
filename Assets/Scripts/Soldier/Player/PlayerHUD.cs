using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Singleton;
    [SerializeField] private Stat m_healthStat;
    [SerializeField] private Stat m_energyStat;
    private GameObject m_hud;
    private Slider m_healthSlider;
    private Slider m_energySlider;

    private void Awake()
    {
        InitializeSingleton();
        Singleton.m_hud = GameObject.FindGameObjectWithTag("HUD");
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
        m_healthSlider = GameObject.Find("HealthSlider").GetComponent<Slider>();
        m_energySlider = GameObject.Find("EnergySlider").GetComponent<Slider>();
    }

    public static void UpdateHUD()
    {
        Singleton.m_healthSlider.value = Singleton.m_healthStat.GetValue();
        Singleton.m_energySlider.value = Singleton.m_energyStat.GetValue();
    }

    public static void SetSeed(string seed)
    {
        Singleton.m_hud.GetComponent<TextMeshProUGUI>().text = seed;
    }
}
