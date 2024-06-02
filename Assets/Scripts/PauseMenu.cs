using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject hudDisplay;
    [SerializeField] private GameObject settings;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }

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
        if (!GameIsPaused) return; 
        pauseMenuUI.SetActive(false);
        hudDisplay.SetActive(true);
        ExitSettings();
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Pause()
    {
        if (GameIsPaused) return;
        pauseMenuUI.SetActive(true);
        hudDisplay.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void ExitSettings()
    {
        settings.SetActive(false);
    }
    public void LoadMenu()
    {
        settings.SetActive(true);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        levelManager.ReturnToMenu();
    }

    // Method to restart the current level
    public void RestartLevel()
    {
        levelManager.RestartLevel();
        Resume();
    }

    // Method to resume the game from the settings menu
    //public static void ResumeGameFromSettings()
    //{
    //    if (GameIsPaused)
    //    {
    //        GameObject pauseMenu = GameObject.FindWithTag("PauseMenu");
    //        if (pauseMenu != null)
    //        {
    //            PauseMenu pauseMenuScript = pauseMenu.GetComponent<PauseMenu>();
    //            if (pauseMenuScript != null)
    //            {
    //                pauseMenuScript.Resume();
    //            }
    //        }
    //    }
    //}
}