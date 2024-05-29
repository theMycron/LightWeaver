using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<string> levels = new List<string>();
    [SerializeField] private AudioManager audioManager;
    private int currentLevel = -1;

    private void Start()
    {
        PlayLevel(0);
        audioManager.PlayMusic(AudioManager.MusicEnum.Level);
    }

    public void OnLevelDone(Component sender, int objectNum, string channel, object data)
    {
        if (channel != "Level") return;
        NextLevel();
    }
    public void OnPlayLevel(Component sender, int objectNum, string channel, object data)
    {
        if (channel != "Level") return;
        PlayLevel(objectNum);
    }
    private void NextLevel()
    {
        Debug.Log("Going to next level. Currently: "+currentLevel);
        if (currentLevel >= levels.Count)
        {
            // no levels after
            return;
        }
        string nextLevel = levels[currentLevel + 1];

        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);

        if (currentLevel >= 0)
        {
            StartCoroutine(UnloadCurrentLevel());
        } else
        {
            Debug.Log("Not unloading. Current scene is "+currentLevel);
        }
        currentLevel++;
    }

    private void PlayLevel(int level)
    {
        Debug.Log($"Going to level {level}. Currently: " + currentLevel);
        if (level >= levels.Count || level == currentLevel)
        {
            return;
        }

        string nextLevel = levels[level];
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
        Debug.Log("Loaded next scene: " + nextLevel);

        if (currentLevel >= 0)
        {
            UnloadCurrentLevel();
            Debug.Log("Unload complete");
        }
        else
        {
            Debug.Log("Not unloading. Current scene is " + currentLevel);
        }

        currentLevel = level;
    }

    private IEnumerator UnloadCurrentLevel()
    {
        Debug.Log("Unloading scene " + levels[currentLevel]);
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levels[currentLevel]);
        Resources.UnloadUnusedAssets();
        yield return ao;
    }
}
