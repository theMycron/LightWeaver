using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("----------- Audio Clip -----------")]
    public AudioClip background;
   


    private void start() {
        
        musicSource.clip = background;
        musicSource.Play();
    
    }

    public void PlaySFX(AudioClip clip) {

        SFXSource.PlayOneShot(clip);
    }


}
