using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevelScene;


    public void OnLevelDone(Component sender, int objectNum, string channel, object data)
    {
        if (channel != "Level") return;
        NextLevel();
    }
    private void NextLevel()
    {
        if (nextLevelScene == null || nextLevelScene.Length == 0) return;
        SceneManager.LoadScene(nextLevelScene);
    }
}
