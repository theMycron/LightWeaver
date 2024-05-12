using UnityEngine;
using TMPro; // Required for TextMeshPro
using UnityEngine.UI;


public class PromptManager : MonoBehaviour
{
    public GameObject promptPrefab; // Assign your prompt prefab here
    public Transform promptBar; // Assign your PromptBar transform here

    void Start()
    {
        // Assuming PromptManager is attached to the same GameObject as this script
        PromptManager promptManager = GetComponent<PromptManager>();
        promptManager.AddPrompt("Drop Cube", PromptIcons.Drop);
    }

    public enum PromptIcons
    {
        Drop,
        Pick,
        Point,
        Rotate
        // Add more icons as needed
    }

    public void AddPrompt(string text, PromptIcons icon)
    {
        // Instantiate the prompt prefab
        GameObject newPrompt = Instantiate(promptPrefab, promptBar);

        // Set the text
        TextMeshProUGUI textMeshPro = newPrompt.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found on the prompt prefab.");
            return;
        }

        // Set the icon
        Image iconImage = newPrompt.GetComponent<Image>();
        switch (icon)
        {
            case PromptIcons.Drop:
                iconImage.sprite = Resources.Load<Sprite>("Assets/Icons/Drop.png"); // Load your icon sprite
                break;
            case PromptIcons.Pick:
                iconImage.sprite = Resources.Load<Sprite>("Assets/Icons/Pick.png");
                break;
            case PromptIcons.Point:
                iconImage.sprite = Resources.Load<Sprite>("Assets/Icons/Point.png"); 
                break;
            case PromptIcons.Rotate:
                iconImage.sprite = Resources.Load<Sprite>("Assets/Icons/Rotate.png"); 
                break;
            // Add cases for other icons as needed
            default:
                Debug.LogError("Unknown icon: " + icon);
                break;
        }
    }
}