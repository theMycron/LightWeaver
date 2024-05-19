using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalTextManager : MonoBehaviour
{
    public TextMeshProUGUI temporaryTextPrefab;
    public Transform textContainer;
    public float displayDuration = 10f;
    public float lineDelay = 0.5f;
    public float characterDelay = 0.05f;
    public float verticalSpacing = 10f;
    public float removalDelay = 0.1f;

    private List<TextMeshProUGUI> activeTexts = new List<TextMeshProUGUI>();

    private void Start()
    {
        string[] messages = { "Try to move Roboot 1...", "Use the box...", "Drop the box..." };
        StartCoroutine(DisplayLinesCoroutine(messages));
        AddNewLine(messages[0]);
    }

    private IEnumerator DisplayLinesCoroutine(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            TextMeshProUGUI temporaryText = Instantiate(temporaryTextPrefab, textContainer);
            temporaryText.text = "";

            activeTexts.Insert(0, temporaryText);
            RepositionTexts();

            for (int j = 0; j < line.Length; j++)
            {
                temporaryText.text += line[j];
                yield return new WaitForSeconds(characterDelay);
            }

            yield return new WaitForSeconds(lineDelay);
        }

        yield return new WaitForSeconds(displayDuration);

        StartCoroutine(RemoveLinesCoroutine());
    }

    private IEnumerator RemoveLinesCoroutine()
    {
        for (int i = activeTexts.Count - 1; i >= 0; i--)
        {
            TextMeshProUGUI text = activeTexts[i];
            activeTexts.RemoveAt(i);

            TMP_TextInfo textInfo = text.textInfo;
            int characterCount = textInfo.characterCount;

            Color32[] newVertexColors = textInfo.meshInfo[0].colors32;
            float fadeOutDuration = 1.0f; // Duration of fade-out animation in seconds
            float elapsedTime = 0.0f;

            while (elapsedTime < fadeOutDuration)
            {
                float alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeOutDuration);

                for (int j = 0; j < characterCount; j++)
                {
                    int materialIndex = textInfo.characterInfo[j].materialReferenceIndex;
                    int vertexIndex = textInfo.characterInfo[j].vertexIndex;

                    // Apply alpha to each vertex color
                    for (int k = 0; k < 4; k++)
                    {
                        int vertexOffset = vertexIndex + k;
                        newVertexColors[vertexOffset].a = (byte)(newVertexColors[vertexOffset].a * alpha);
                    }
                }

                // Update the vertex colors and mesh
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                text.UpdateMeshPadding();

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Ensure the text is completely faded out
            for (int j = 0; j < characterCount; j++)
            {
                int vertexIndex = textInfo.characterInfo[j].vertexIndex;

                // Set alpha to zero for each vertex color
                for (int k = 0; k < 4; k++)
                {
                    int vertexOffset = vertexIndex + k;
                    newVertexColors[vertexOffset].a = 0;
                }
            }

            // Update the vertex colors and mesh again
            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            text.UpdateMeshPadding();

            // Wait for a short delay before destroying the text object
            yield return new WaitForSeconds(removalDelay);

            Destroy(text.gameObject);
            RepositionTexts();
        }
    }

    private void RepositionTexts()
    {
        float totalHeight = 0f;

        for (int i = 0; i < activeTexts.Count; i++)
        {
            totalHeight += activeTexts[i].rectTransform.sizeDelta.y + verticalSpacing;
        }

        // Resize the background image
        Vector2 backgroundSize = new Vector2(textContainer.GetComponent<RectTransform>().rect.width, totalHeight);
        textContainer.GetComponent<RectTransform>().sizeDelta = backgroundSize;
    }

    public void AddNewLine(string message)
    {
        TextMeshProUGUI temporaryText = Instantiate(temporaryTextPrefab, textContainer);
        temporaryText.text = "";

        activeTexts.Insert(0, temporaryText);
        RepositionTexts();

        StartCoroutine(DisplayLineCoroutine(temporaryText, message));
        RepositionTexts();
    }

    private IEnumerator DisplayLineCoroutine(TextMeshProUGUI textObject, string line)
    {
        for (int j = 0; j < line.Length; j++)
        {
            textObject.text += line[j];
            yield return new WaitForSeconds(characterDelay);
        }

        yield return new WaitForSeconds(lineDelay);
    }
}