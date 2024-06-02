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
    public string parameterName;
    public Slider masterVol, musicVol, sfxVol;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullScreenToggle;

    Resolution[] resolutions;
    private static MenuOrigin menuOrigin;

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
        SetSavedVolume();

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

    public void SetSavedVolume()
    {
        if (PlayerPrefs.HasKey("MasterVol"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVol");
            masterVol.value = savedVolume;
        }
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVol");
            musicVol.value = savedVolume;
        }
        if (PlayerPrefs.HasKey("SfxVol"))
        {
            float savedVolume = PlayerPrefs.GetFloat("SfxVol");
            musicVol.value = savedVolume;
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
        PlayerPrefs.SetFloat("MasterVol", masterVolume);
        PlayerPrefs.SetFloat("MusicVol", musicVolume);
        PlayerPrefs.SetFloat("SfxVol", sfxVolume);
        PlayerPrefs.Save(); // Save PlayerPrefs
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
        float defaultVolume = -10.0f; // Set your desired default volume value
        masterVol.value = defaultVolume;
        musicVol.value = defaultVolume;
        sfxVol.value = defaultVolume;
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

}