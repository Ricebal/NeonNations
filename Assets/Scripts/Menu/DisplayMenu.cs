/**
 * Authors: Stella
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMenu : MonoBehaviour
{
    [SerializeField] private Dropdown m_resolutionDropdown = null;
    [SerializeField] private Toggle m_fullScreenToggle = null;
    [SerializeField] private Dropdown m_qualityDropdown = null;
    // Text displayed when the settings are saved
    [SerializeField] private TextMeshProUGUI m_savingText = null;

    // List of resolutions
    private List<Resolution> m_resolutions = new List<Resolution>();

    private int m_isFullScreen;

    private void Start()
    {
        // List of resolutions converted to string in order to add them to the dropdown
        List<string> options = new List<string>();
        // Index in the resolutions list of the current resolution of the window
        int currentResolutionIndex = 0;

        // Resolutions available for the screen
        Resolution[] unityResolutions = Screen.resolutions;
        for (int i = 0; i < unityResolutions.Length; i++)
        {
            if (unityResolutions[i].width >= 800 && unityResolutions[i].height >= 600)
            {
                m_resolutions.Add(unityResolutions[i]);
            }
        }

        m_resolutionDropdown.ClearOptions();

        // Convert resolutions into strings and add them to the options list
        for (int i = 0; i < m_resolutions.Count; i++)
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

    private void SetResolution(int resolutionIndex)
    {
        Resolution resolution = m_resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    private void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        m_isFullScreen = isFullScreen ? 1 : 0;
    }

    // Apply the settings selected by the user
    public void Apply()
    {
        SetResolution(m_resolutionDropdown.value);
        SetFullScreen(m_fullScreenToggle.isOn);
        SetQuality(m_qualityDropdown.value);
        StartCoroutine(ShowMessage("Options saved", 1));
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
