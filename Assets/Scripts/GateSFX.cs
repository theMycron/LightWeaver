using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateSFX : MonoBehaviour
{
    [SerializeField] AudioSource AudioSource;

    public AudioClip gateOpen;
    public AudioClip gateClosed;

    [Header("Pitch Variation")]
    public float minPitch = 0.8f; // Minimum pitch value
    public float maxPitch = 1.2f; // Maximum pitch value

    public void PlayGateSFX(AudioClip clip)
    {
        AudioSource.pitch = Random.Range(minPitch, maxPitch);
        AudioSource.clip = clip;
        AudioSource.Play();
    }

    public void PlayOpenSound()
    {
        PlayGateSFX(gateOpen);
    }

    public void PlayCloseSound()
    {
        PlayGateSFX(gateClosed);
    }

}
