using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    private LevelManager levelManager;
    void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    public void StartLevel(int level)
    {
        levelManager.OnPlayLevel(this,level,"Level",null);
    }
}
