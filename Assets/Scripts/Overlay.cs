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
    }

    public void StartFadeIn()
    {
        image.canvasRenderer.SetAlpha(1f);
        image.CrossFadeAlpha(0, fadeTime, false);
    }
    public void StartFadeOut()
    {
        image.canvasRenderer.SetAlpha(0);
        image.CrossFadeAlpha(1f, fadeTime, false);
    }
}
