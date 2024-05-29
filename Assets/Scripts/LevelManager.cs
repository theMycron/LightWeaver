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
        if (currentLevel >= levels.Count)
        {
            // no levels after
            return;
        }
        if (currentLevel >= 0)
        {
            UnloadCurrentLevel();
        }

        string nextLevel = levels[currentLevel + 1];

        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
    }

    private void PlayLevel(int level)
    {
        if (level >= levels.Count || level == currentLevel)
        {
            return;
        }
        if (currentLevel >= 0)
            SceneManager.UnloadSceneAsync(levels[currentLevel]);

        string nextLevel = levels[level];

        SceneManager.LoadScene(nextLevel, LoadSceneMode.Additive);
    }

    private IEnumerator UnloadCurrentLevel()
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levels[currentLevel]);
        yield return ao;

    }
}
