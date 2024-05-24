using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public string parameterName;
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullScreenToggle;

    Resolution[] resolutions;

    private void Awake()
    {
        // Initialize resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        Debug.Log($"Initial Resolution: {resolutions[currentResolutionIndex].width} x {resolutions[currentResolutionIndex].height}");
    }

    private void Start()
    {
        // Load the saved volume from PlayerPrefs
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
        else
        {
            // Initialize the volume slider to the current audio mixer volume
            if (audioMixer.GetFloat(parameterName, out float currentVolume))
            {
                volumeSlider.value = Mathf.Pow(10, currentVolume / 20); // Convert dB to linear
            }
            else
            {
                Debug.LogWarning("Could not get the volume parameter from the Audio Mixer.");
            }
        }

        // Initialize the graphics dropdown
        int currentQualityIndex = QualitySettings.GetQualityLevel();
        if (PlayerPrefs.HasKey("QualityIndex"))
        {
            currentQualityIndex = PlayerPrefs.GetInt("QualityIndex");
        }

        graphicsDropdown.value = currentQualityIndex;
        graphicsDropdown.RefreshShownValue();

        // Initialize the full screen toggle
        fullScreenToggle.isOn = Screen.fullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        Debug.Log($"Resolution set to: {resolution.width} x {resolution.height}");
    }

    public void SetVolume(float volume)
    {
        float dB = Mathf.Log10(volume) * 20;
        audioMixer.SetFloat(parameterName, dB);
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save(); // Save PlayerPrefs
        Debug.Log($"Setting volume to {dB} dB");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityIndex", qualityIndex);
        PlayerPrefs.Save();
        Debug.Log($"Setting quality to {QualitySettings.names[qualityIndex]}");
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }


    public void ResetSettings()
    {
        // Reset volume
        float defaultVolume = 0.0f; // Set your desired default volume value
        volumeSlider.value = defaultVolume;
        SetVolume(defaultVolume);

        // Reset resolution
        int defaultResolutionIndex = resolutions.Length - 1; // Set the index of the last resolution in the options list
        SetResolution(defaultResolutionIndex);

        // Reset graphics quality
        int defaultQualityIndex = 2; // Set the index of the default quality level
        SetQuality(defaultQualityIndex);

        // Reset full screen
        bool defaultFullScreen = true; // Set the default full screen value
        SetFullScreen(defaultFullScreen);

        // Check fullscreen toggle
        fullScreenToggle.isOn = defaultFullScreen;
    }

}