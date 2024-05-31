using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----------- Audio Source -----------")]
    [SerializeField] private AudioSource MusicPlayer;
    [SerializeField] private AudioSource SFXSource;

    [Header("----------- Music -----------")]
    [SerializeField] private AudioClip mainMenu;
    [SerializeField] private AudioClip level;
    [SerializeField] private AudioClip end;


    public enum MusicEnum
    {
        MainMenu = 1,
        Level = 2,
        End = 3
    }


    public void PlayMusic(MusicEnum track)
    {
        MusicPlayer.Stop();
        switch (track)
        {
            case MusicEnum.MainMenu:
                MusicPlayer.clip = mainMenu;
                break;
                case MusicEnum.Level:
                MusicPlayer.clip = level;
                break;
                case MusicEnum.End:
                MusicPlayer.clip = end;
                break;
        }
        MusicPlayer.Play();
    }
    
    public void PlaySFX(AudioClip clip) {

        SFXSource.PlayOneShot(clip);
    }
}
