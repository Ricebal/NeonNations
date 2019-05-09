using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Dropdown ResolutionDropdown;
    public Toggle FullScreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        resolutions = Screen.resolutions;
        ResolutionDropdown.ClearOptions();

        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.value = currentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();

        FullScreenToggle.isOn = Screen.fullScreen;
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
    }
}
