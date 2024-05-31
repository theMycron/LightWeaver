using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<string> levels = new List<string>();
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private Overlay overlay;
    private int currentLevel = -1;

    public SaveProfile gameSave;

    private void Awake()
    {
        gameSave = SaveManager.Load();
    }
    private void Start()
    {
        // loads main menu at start
        ReturnToMenu();
    }

    public void OnLevelDone(Component sender, int objectNum, string channel, object data)
    {
        if (channel != "Level") return;
        StartCoroutine(NextLevel());
    }
    public void OnPlayLevel(Component sender, int objectNum, string channel, object data)
    {
        if (channel != "Level") return;
        StartCoroutine(PlayLevel(objectNum));
    }

    public void RestartLevel()
    {
        StartCoroutine(PlayLevel(currentLevel));
    }

    public void ReturnToMenu()
    {
        LoadScene("MainMenu");

        StartCoroutine(UnloadScene("HUD"));
        if (currentLevel >= 0)
            StartCoroutine(UnloadScene(levels[currentLevel]));

        audioManager.PlayMusic(AudioManager.MusicEnum.MainMenu);
    }

    public void ContinueGame()
    {
        // will start at 0 if there is no save
        // if there is a save, start at the last level unlocked
        int level = 0;
        if (gameSave != null)
            level = gameSave.lastUnlockedLevel;
        ExitMainMenu();
        StartCoroutine(PlayLevel(level));
    }

    private void ExitMainMenu()
    {
        StartCoroutine(UnloadScene("MainMenu"));
        LoadScene("HUD");
        audioManager.PlayMusic(AudioManager.MusicEnum.Level);
    }

    private IEnumerator NextLevel()
    {
        Debug.Log("Going to next level. Currently: "+currentLevel);
        if (currentLevel >= levels.Count)
        {
            // no levels after
            yield break;
        }
        string nextLevel = levels[currentLevel + 1];
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
        if (currentLevel >= 0)
        {
            StartCoroutine(UnloadScene(levels[currentLevel]));
        } else
        {
            Debug.Log("Not unloading. Current scene is "+currentLevel);
        }
        currentLevel++;
        overlay.StartFadeIn();

        // update last level unlocked

        if (gameSave == null)
            gameSave = new SaveProfile { lastUnlockedLevel = 0 };

        if (gameSave.lastUnlockedLevel < currentLevel)
        {
            gameSave.lastUnlockedLevel = currentLevel;
            SaveManager.Save(gameSave);
        }
    }

    private IEnumerator PlayLevel(int level)
    {
        Debug.Log($"Going to level {level}. Currently: " + currentLevel);
        if (level >= levels.Count)
        {
            yield break;
        }

        string nextLevel = levels[level];
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
        Debug.Log("Loaded next scene: " + nextLevel);

        if (currentLevel >= 0)
        {
            StartCoroutine(UnloadScene(levels[currentLevel]));
            Debug.Log("Unload complete");
        }
        else
        {
            Debug.Log("Not unloading. Current scene is " + currentLevel);
        }

        currentLevel = level;
        overlay.StartFadeIn();
    }

    private void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
    private IEnumerator UnloadScene(string sceneName)
    {
        // get scene and dont unload if invalid
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.IsValid()) yield break;

        AsyncOperation ao = SceneManager.UnloadSceneAsync(scene);
        Resources.UnloadUnusedAssets();
        yield return ao;
    }
    private IEnumerator FadeOut()
    {
        overlay.StartFadeOut();
        yield return new WaitForSeconds(overlay.fadeTime);
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1);
        overlay.StartFadeIn();
    }
}
