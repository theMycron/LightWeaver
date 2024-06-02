using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
//using static System.Net.Mime.MediaTypeNames;

public class PromptManager : MonoBehaviour
{
    public GameObject promptPrefab;
    //public Transform[] promptPositions; // Array of Transform markers for static positions
    public GameObject promptParent;
    private bool promptsAdded = false; // Flag to track if prompts have been added
    [SerializeField] private GameObject grid;

    void Start()
    {
        if (!promptsAdded)
        {
            AddPrompt("Pick Cube", PromptIcons.Pick);
            promptsAdded = true;
        }
    }

    public enum PromptIcons
    {
        None,
        Drop,
        Pick,
        Point,
        Rotate
        // Add more icons as needed
    }

    public Sprite dropSprite; // Reference to the Drop sprite in the Unity Inspector
    public Sprite pickSprite; // Reference to the Pick sprite in the Unity Inspector
    public Sprite pointSprite; // Reference to the Point sprite in the Unity Inspector
    public Sprite rotateSprite; // Reference to the Rotate sprite in the Unity Inspector

    public GameObject AddPrompt(string text, PromptIcons icon)
    {
        // add to grid
        GameObject newPrompt = Instantiate(promptPrefab);
        newPrompt.transform.SetParent(grid.transform);

        TextMeshProUGUI textMeshPro = newPrompt.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }

        Transform iconObject = newPrompt.transform.Find("icon");
        if (iconObject != null)
        {
            Image iconImage = iconObject.GetComponent<Image>();
            if (iconImage != null)
            {
                if (icon == PromptIcons.None)
                {
                    Destroy(iconObject.gameObject);
                }
                else
                {
                    Sprite iconSprite = null;

                    switch (icon)
                    {
                        case PromptIcons.Drop:
                            iconSprite = dropSprite;
                            break;
                        case PromptIcons.Pick:
                            iconSprite = pickSprite;
                            break;
                        case PromptIcons.Point:
                            iconSprite = pointSprite;
                            break;
                        case PromptIcons.Rotate:
                            iconSprite = rotateSprite;
                            break;
                        default:
                            Debug.LogError("Unhandled prompt icon: " + icon.ToString());
                            break;
                    }

                    if (iconSprite != null)
                    {
                        iconImage.sprite = iconSprite;
                    }
                    else
                    {
                        Debug.LogError("Icon sprite not set for: " + icon.ToString());
                    }
                }
            }
            else
            {
                Debug.LogError("Image component not found on the Icon object.");
                Destroy(newPrompt); // Clean up the instantiated object if there's an error
                return null;
            }
        }
        else if (icon != PromptIcons.None)
        {
            Debug.LogError("Icon object not found in the prompt prefab.");
            Destroy(newPrompt); // Clean up the instantiated object if there's an error
            return null;
        }
        return newPrompt;
    }

    public void RemovePrompt(GameObject prompt)
    {
        if (prompt != null)
        {
            Destroy(prompt);
        }
        else
        {
            Debug.LogWarning("Attempted to remove a null prompt.");
        }
    }

    public void ClearPrompts()
    {
        foreach (var item in promptParent.GetComponentsInChildren<GameObject>())
        {
            Destroy(item.gameObject);
        }
    }
}