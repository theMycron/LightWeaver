using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject levelButton;

    private LevelManager levelManager;
    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
        CreateButtons();
    }

    // add buttons to the grid object, unlocking levels that are unlocked
    private void CreateButtons()
    {
        int levelCount = levelManager.LevelCount;
        int lastUnlockedLevel = levelManager.gameSave != null ? levelManager.gameSave.lastUnlockedLevel : 0;

        // unlock the levels up to the point the player has reached
        for (int i = 0;i < levelCount; i++)
        {
            // add to grid
            GameObject newButton = Instantiate(levelButton);
            newButton.transform.SetParent(grid.transform);
            LevelSelectButton script = newButton.GetComponent<LevelSelectButton>();
            // unlock 
            if (i <= lastUnlockedLevel)
            {
                script.Unlock(i);
            } else
            {
                script.SetNumber(i);
            }
        }
    }
}
