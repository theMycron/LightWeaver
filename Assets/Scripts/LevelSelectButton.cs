using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private static Color unlockedColor = new Color(153,0,226,100);

    private Button btn;
    private TextMeshProUGUI text;
    private Image backImage;
    private LevelManager levelManager;

    private void Awake()
    {
        btn = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        backImage = GetComponentInChildren<Image>();
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    public void SetNumber(int num)
    {
        text.text = num.ToString();
    }

    public void Unlock(int levelNum)
    {
        btn.onClick.AddListener(delegate { PlayLevel(levelNum); });
        SetNumber(levelNum);
        backImage.color = unlockedColor;
    }

    public void PlayLevel(int level)
    {
        levelManager.SelectLevel(level);
    }
}
