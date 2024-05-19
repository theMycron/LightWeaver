using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PromptManager : MonoBehaviour
{
    public GameObject promptPrefab;
    public Transform promptBar;

    void Start()
    {

        // Example calls to the AddPrompt method
        AddPrompt("Drop Cube",PromptIcons.Drop);
        AddPrompt("Pick Cube",PromptIcons.Pick);
        AddPrompt("Rotate Object", PromptIcons.Rotate);
        AddPrompt("Custom Text", PromptIcons.Point);
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


    public void AddPrompt(string text, PromptIcons icon)
    {
        if (string.IsNullOrEmpty(text) && icon == PromptIcons.None)
        {
            Debug.LogWarning("Both text and icon are empty. Skipping prompt creation.");
            return;
        }

        GameObject newPrompt = Instantiate(promptPrefab, promptBar);

        float promptHeight = newPrompt.GetComponent<RectTransform>().rect.height;
        Vector3 promptPosition;
        int promptCount = promptBar.childCount;

        if (promptCount == 0)
        {
            // Position the first prompt at the same location as the prefab
            promptPosition = Vector3.zero;
        }
        else
        {
            // Position subsequent prompts on top of each other
            float stackedHeight = promptHeight * promptCount;
            promptPosition = new Vector3(0f, stackedHeight, 0f);
        }

        newPrompt.transform.localPosition = promptPosition;

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

        Transform iconObject = newPrompt.transform.Find("Icon");
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
    