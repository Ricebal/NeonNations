using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private Dropdown m_resolutionDropdown;
    [SerializeField]
    private Toggle m_fullScreenToggle;
    [SerializeField]
    private Dropdown m_qualityDropdown;
    // Text displayed when the settings are saved
    [SerializeField]
    private TextMeshProUGUI m_savingText;

    // List of resolutions
    private Resolution[] m_resolutions;

    private static string s_resolutionWidth = "resolutionWidth";
    private static string s_resolutionHeight = "resolutionHeight";
    private static string s_quality = "quality";
    private static string s_fullScreen = "fullScreen";

    private int m_isFullScreen;

    void Awake()
    {
        LoadSavedValues();
    }

    void Start()
    {
        // List of resolutions converted to string in order to add them to the dropdown
        List<string> options = new List<string>();
        // Index in the resolutions list of the current resolution of the window
        int currentResolutionIndex = 0;

        // Resolutions available for the screen
        m_resolutions = Screen.resolutions;

        m_resolutionDropdown.ClearOptions();

        // Convert resolutions into strings and add them to the options list
        for (int i = 0; i < m_resolutions.Length; i++)
        {
            options.Add(m_resolutions[i].width + " x " + m_resolutions[i].height);

            if (m_resolutions[i].width == Screen.width && m_resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        m_resolutionDropdown.AddOptions(options);
        m_resolutionDropdown.value = currentResolutionIndex;
        m_resolutionDropdown.RefreshShownValue();

        m_fullScreenToggle.isOn = Screen.fullScreen;

        m_qualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = m_resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        m_isFullScreen = isFullScreen ? 1 : 0;
    }

    // Save the settings selected by the user
    public void Save()
    {
        PlayerPrefs.SetInt(s_resolutionWidth, Screen.width);
        PlayerPrefs.SetInt(s_resolutionHeight, Screen.height);
        PlayerPrefs.SetInt(s_quality, QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt(s_fullScreen, m_isFullScreen);
        StartCoroutine(ShowMessage("Options saved", 1));
    }

    // Load and apply the settings saved by the user
    public void LoadSavedValues()
    {
        if (PlayerPrefs.HasKey(s_fullScreen) && PlayerPrefs.HasKey(s_resolutionWidth) && PlayerPrefs.HasKey(s_resolutionHeight) && PlayerPrefs.HasKey(s_quality))
        {
            bool isFullScreen;

            m_isFullScreen = PlayerPrefs.GetInt(s_fullScreen);
            isFullScreen = m_isFullScreen != 0;

            Screen.SetResolution(PlayerPrefs.GetInt(s_resolutionWidth), PlayerPrefs.GetInt(s_resolutionHeight), isFullScreen);
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(s_quality));
        }
    }

    // Show the given message for the given delay in seconds
    IEnumerator ShowMessage(string message, float delay)
    {
        m_savingText.text = message;
        m_savingText.enabled = true;
        yield return new WaitForSeconds(delay);
        m_savingText.enabled = false;
    }
}
