using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overlay : MonoBehaviour
{
    [SerializeField] public float fadeTime;
    private RawImage image;

    void Start()
    {
        image = GetComponentInChildren<RawImage>();
        image.color = Color.black;
        image.canvasRenderer.SetAlpha(0);
    }

    public void StartFadeIn()
    {
        image.color = Color.black;
        image.canvasRenderer.SetAlpha(1f);
        image.CrossFadeAlpha(0, fadeTime, false);
    }
    public void StartFadeOut()
    {
        image.color = Color.black;
        image.canvasRenderer.SetAlpha(0);
        image.CrossFadeAlpha(1f, fadeTime, false);
    }

    public void FadeToWhite()
    {
        image.color = Color.white;
        image.canvasRenderer.SetAlpha(0f);
        image.CrossFadeAlpha(1f, fadeTime*2, false);
    }
    public void FadeFromWhite()
    {
        Debug.Log("Fading back");
        image.color = Color.white;
        image.canvasRenderer.SetAlpha(1f);
        image.CrossFadeAlpha(0f, fadeTime*2, false);
    }
}
