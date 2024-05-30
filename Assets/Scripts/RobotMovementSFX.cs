using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    [SerializeField] AudioSource AudioSource;
    [SerializeField] AudioClip[] footstepSounds;

    public AudioClip JumpingSound;
    public AudioClip landingSound;

    [Header("Pitch Variation")]
    public float minPitch = 0.8f; // Minimum pitch value
    public float maxPitch = 1.2f; // Maximum pitch value
    public void PlayFootstepSounds()
    {
        // Play a random footstep sound from the array
        AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
        AudioSource.PlayOneShot(footstepSound);

    }
    public void PlayRobotSFX(AudioClip clip)
    {
        AudioSource.pitch = Random.Range(minPitch, maxPitch);
        AudioSource.PlayOneShot(clip);
    }

    public void PlayLandingSound()
    {
        PlayRobotSFX(landingSound);
    }

    public void PlayJumpingSound()
    {
        PlayRobotSFX(JumpingSound);
    }


}


