using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject hudDisplay;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        hudDisplay.SetActive(true);
        Time.timeScale = 0.5f;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        hudDisplay.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 0.5f;
        SettingsMenu.SetMenuOrigin(MenuOrigin.PauseMenu);
        SceneManager.LoadScene("Settings");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // Method to restart the current level
    public void RestartLevel()
    {
        Time.timeScale = 1f;  // Ensure the game time is running
        string currentSceneName = SceneManager.GetActiveScene().name;  // Get the current scene name
        SceneManager.LoadScene(currentSceneName);  // Reload the current scene
    }

    // Method to resume the game from the settings menu
    public static void ResumeGameFromSettings()
    {
        if (GameIsPaused)
        {
            GameObject pauseMenu = GameObject.FindWithTag("PauseMenu");
            if (pauseMenu != null)
            {
                PauseMenu pauseMenuScript = pauseMenu.GetComponent<PauseMenu>();
                if (pauseMenuScript != null)
                {
                    pauseMenuScript.Resume();
                }
            }
        }
    }
}