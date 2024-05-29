using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public  AudioClip background;
    public  AudioClip gateOpen;
    public  AudioClip Jumping;
    public  AudioClip walking;
    public  AudioClip cubePickup;
    public  AudioClip robotSwitch;
    public  AudioClip laserReceiver;
    public  AudioClip floorButton;
    public  AudioClip collectible;
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
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

}
