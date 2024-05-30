using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    private void start() {
        
        musicSource.clip = mainMenu;
        musicSource.Play();
    
    }



    [Header("----------- Music -----------")]
    [SerializeField] private AudioClip mainMenu;
    [SerializeField] private AudioClip level;
    [SerializeField] private AudioClip end;

    private AudioSource player;

    public enum MusicEnum
    {
        MainMenu = 1,
        Level = 2,
        End = 3
    }

    private void Awake()
    {
        player = GetComponent<AudioSource>();
    }

    public void PlayMusic(MusicEnum track)
    {
        player.Stop();
        switch (track)
        {
            case MusicEnum.MainMenu:
                player.clip = mainMenu;
                break;
                case MusicEnum.Level:
                player.clip = level;
                break;
                case MusicEnum.End:
                player.clip = end;
                break;
        }
        player.Play();
    }
    
    public void PlaySFX(AudioClip clip) {

        SFXSource.PlayOneShot(clip);
    }
}
