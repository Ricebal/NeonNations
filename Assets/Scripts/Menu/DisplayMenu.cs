using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMenu : MonoBehaviour
{
    [SerializeField] private Dropdown m_resolutionDropdown;
    [SerializeField] private Toggle m_fullScreenToggle;
    [SerializeField] private Dropdown m_qualityDropdown;
    // Text displayed when the settings are saved
    [SerializeField] private TextMeshProUGUI m_savingText;

    private readonly int[] RESOLUTIONS_TO_SHOW = new int[] { 6, 7, 8, 10, 11, 13, 15, 16, 17, 18 };

    // List of resolutions
    private Resolution[] m_resolutions;

    private int m_isFullScreen;

    private void Start()
    {
        // List of resolutions converted to string in order to add them to the dropdown
        List<string> options = new List<string>();
        // Index in the resolutions list of the current resolution of the window
        int currentResolutionIndex = 0;

        // Resolutions available for the screen
        m_resolutions = Screen.resolutions;

        m_resolutionDropdown.ClearOptions();

        // Convert resolutions into strings and add them to the options list
        for (int i = 0; i < RESOLUTIONS_TO_SHOW.Length; i++)
        {
            options.Add(m_resolutions[RESOLUTIONS_TO_SHOW[i]].width + " x " + m_resolutions[RESOLUTIONS_TO_SHOW[i]].height);

            if (m_resolutions[RESOLUTIONS_TO_SHOW[i]].width == Screen.width && m_resolutions[RESOLUTIONS_TO_SHOW[i]].height == Screen.height)
            {
                currentResolutionIndex = RESOLUTIONS_TO_SHOW[i];
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
        Resolution resolution = m_resolutions[RESOLUTIONS_TO_SHOW[resolutionIndex]];
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
