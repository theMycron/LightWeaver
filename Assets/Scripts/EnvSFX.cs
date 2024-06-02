using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvSFX : MonoBehaviour
{
    public static EnvSFX instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource ObjectsSFX;
    [SerializeField] AudioSource chargingSFX;
    [Header("Audio Clips")]

    public AudioClip cubePickup;
    public AudioClip robotSwitch;
    public AudioClip collectible;


    public AudioClip riserSound;
    public AudioClip activationSound;
    


    [Header("Pitch Variation")]
    public float minPitch = 0.8f; // Minimum pitch value
    public float maxPitch = 1.2f; // Maximum pitch value

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayObjectSFX(AudioClip clip)
    {
        ObjectsSFX.pitch = Random.Range(minPitch, maxPitch);
        ObjectsSFX.PlayOneShot(clip);
    }

    public void StopObjectSFX()
    {
        ObjectsSFX.pitch = Random.Range(minPitch, maxPitch);
        ObjectsSFX.Stop();
    }

    public void PlayChargingSFX(AudioClip clip)
    {
        chargingSFX.clip = clip;
        chargingSFX.Play();

    }

    public void StopRiserSound()
    {
        if (chargingSFX != null && chargingSFX.clip == riserSound)
        {
            chargingSFX.Stop();
        }
    }
}
