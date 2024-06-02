using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public enum MenuOrigin
{
    MainMenu,
    PauseMenu
}

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterVol, musicVol, sfxVol;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullScreenToggle;

    Resolution[] resolutions;
    private static MenuOrigin menuOrigin;

    private void Awake()
    {
        InitializeResolutions();
    }

    private void Start()
    {
        // Load the saved volume from PlayerPrefs
        SetSavedVolume();

        // Initialize the graphics dropdown
        int currentQualityIndex = QualitySettings.GetQualityLevel();
        if (PlayerPrefs.HasKey("QualityIndex"))
        {
            currentQualityIndex = PlayerPrefs.GetInt("QualityIndex");
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = currentQualityIndex;
            graphicsDropdown.RefreshShownValue();
        }

        // Initialize the full screen toggle
        if (fullScreenToggle != null)
        {
            fullScreenToggle.isOn = Screen.fullScreen;
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionDropdown == null) return;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        Debug.Log($"Resolution set to: {resolution.width} x {resolution.height}");
    }

    public void SetSavedVolume()
    {
        if (PlayerPrefs.HasKey("MasterVol"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVol");
            masterVol.value = savedVolume;
            Debug.Log("Saved Master: " + savedVolume);
        }
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVol");
            musicVol.value = savedVolume;
            Debug.Log("Saved Music: " + savedVolume);
        }
        if (PlayerPrefs.HasKey("SfxVol"))
        {
            float savedVolume = PlayerPrefs.GetFloat("SfxVol");
            sfxVol.value = savedVolume;
            Debug.Log("Saved SFX: " + savedVolume);
        }
        AutoSetVolume();
    }

    // sets volumes for all channels based on slider values and saves it
    public void AutoSetVolume()
    {
        float masterVolume = masterVol.value;
        float musicVolume = musicVol.value;
        float sfxVolume = sfxVol.value;
        float masterdB = Mathf.Log10(masterVolume) * 20;
        float musicdB = Mathf.Log10(musicVolume) * 20;
        float sfxdB = Mathf.Log10(sfxVolume) * 20;
        audioMixer.SetFloat("MasterVol", masterdB);
        audioMixer.SetFloat("MusicVol", musicdB);
        audioMixer.SetFloat("SfxVol", sfxdB);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Debug.Log($"Setting quality to {QualitySettings.names[qualityIndex]}");
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("QualityIndex", QualitySettings.GetQualityLevel());
        PlayerPrefs.SetFloat("MasterVol", masterVol.value);
        PlayerPrefs.SetFloat("MusicVol", musicVol.value);
        PlayerPrefs.SetFloat("SfxVol", sfxVol.value);
        PlayerPrefs.Save(); // Save PlayerPrefs
        Debug.Log("SAVED PREFS");
    }

    public void ResetSettings()
    {
        // Reset volume
        masterVol.value = 1f;
        musicVol.value = 0.4f;
        sfxVol.value = 0.8f;
        AutoSetVolume();

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

        // Check menu origin and handle transition
        //switch (GetMenuOrigin())
        //{
        //    case MenuOrigin.MainMenu:
        //        // Handle transition from main menu
        //        break;
        //    case MenuOrigin.PauseMenu:
        //        // Handle transition from pause menu
        //        PauseMenu.ResumeGameFromSettings();
        //        break;
        //}
    }

    public static void SetMenuOrigin(MenuOrigin origin)
    {
        menuOrigin = origin;
    }

    public static MenuOrigin GetMenuOrigin()
    {
        return menuOrigin;
    }

    private void InitializeResolutions()
    {
        if (resolutionDropdown == null) return;
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
}