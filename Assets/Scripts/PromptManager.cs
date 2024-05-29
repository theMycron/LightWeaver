using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public GameObject promptPrefab;
    public Transform[] promptPositions; // Array of Transform markers for static positions
    public GameObject promptParent;
    private bool promptsAdded = false; // Flag to track if prompts have been added

    void Start()
    {
        if (!promptsAdded)
        {
            AddPrompt("Drop Cube", PromptIcons.Drop, 0); // Pass index to select the position
            AddPrompt("Pick Cube", PromptIcons.Pick, 1);
            AddPrompt("Rotate Object", PromptIcons.Rotate, 2);
            AddPrompt("Point laser", PromptIcons.Point, 3);

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

    public void AddPrompt(string text, PromptIcons icon, int positionIndex)
    {
        if (string.IsNullOrEmpty(text) && icon == PromptIcons.None)
        {
            Debug.LogWarning("Both text and icon are empty. Skipping prompt creation.");
            return;
        }

        // Instantiate the new prompt as a child of promptParent
        GameObject newPrompt = Instantiate(promptPrefab, promptParent.transform);

        // Position the new prompt at the desired position
        newPrompt.transform.position = promptPositions[positionIndex].position;

        TextMeshProUGUI textMeshPro = newPrompt.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found on the prompt prefab.");
            return;
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
                return;
            }
        }
        else if (icon != PromptIcons.None)
        {
            Debug.LogError("Icon object not found in the prompt prefab.");
        }
    }
}