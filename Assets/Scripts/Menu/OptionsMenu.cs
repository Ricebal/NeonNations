﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public Dropdown ResolutionDropdown;
    public Toggle FullScreenToggle;
    public Dropdown QualityDropdown;
    // Text displayed when the settings are saved
    public TextMeshProUGUI SavingText;

    // List of resolutions
    Resolution[] resolutions;

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
        resolutions = Screen.resolutions;

        ResolutionDropdown.ClearOptions();

        // Convert resolutions into strings and add them to the options list
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();

        FullScreenToggle.isOn = Screen.fullScreen;

        QualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
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

    IEnumerator ShowMessage(string message, float delay)
    {
        SavingText.text = message;
        SavingText.enabled = true;
        yield return new WaitForSeconds(delay);
        SavingText.enabled = false;
    }
}
