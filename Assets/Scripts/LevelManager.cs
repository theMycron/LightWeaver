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
    

    private void Start()
    {
        StartCoroutine(PlayLevel(0));
        audioManager.PlayMusic(AudioManager.MusicEnum.Level);
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
    private IEnumerator NextLevel()
    {
        Debug.Log("Going to next level. Currently: "+currentLevel);
        if (currentLevel >= levels.Count)
        {
            // no levels after
            yield return null;
        }
        string nextLevel = levels[currentLevel + 1];
        yield return StartCoroutine(FadeOut());
        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
        if (currentLevel >= 0)
        {
            StartCoroutine(UnloadCurrentLevel());
        } else
        {
            Debug.Log("Not unloading. Current scene is "+currentLevel);
        }
        currentLevel++;
        overlay.StartFadeIn();
    }

    private IEnumerator PlayLevel(int level)
    {
        Debug.Log($"Going to level {level}. Currently: " + currentLevel);
        if (level >= levels.Count || level == currentLevel)
        {
            yield return null;
        }

        string nextLevel = levels[level];
        yield return StartCoroutine(FadeOut());
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
        overlay.StartFadeIn();
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
    private IEnumerator UnloadCurrentLevel()
    {
        Debug.Log("Unloading scene " + levels[currentLevel]);
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levels[currentLevel]);
        Resources.UnloadUnusedAssets();
        yield return ao;
    }
}
