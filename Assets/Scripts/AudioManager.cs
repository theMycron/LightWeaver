using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource ObjectsSFX;
    [SerializeField] AudioSource RobotMovementSFX;

    [Header("Audio Clips")]
    public AudioClip gateOpen;
    public AudioClip gateClosed;
    public AudioClip Jumping;
    public AudioClip landing;

    public AudioClip cubePickup;
    public AudioClip robotSwitch;
    public AudioClip laserReceiver;
    public AudioClip floorButton;
    public AudioClip collectible;

    [Header("FootSteps")]
    public AudioClip[] footstepSounds;
    public float minTimeBetweenFootsteps = 0.3f; // Minimum time between footstep sounds
    public float maxTimeBetweenFootsteps = 0.6f; // Maximum time between footstep sounds

    public bool isWalking = false; // Flag to track if the player is walking
    private float timeSinceLastFootstep; // Time since the last footstep sound


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
    private void Update()
    {
       /* PlayFootstepSounds();*/
    }

    public void PlayObjectSFX(AudioClip clip)
    {
        ObjectsSFX.pitch = Random.Range(minPitch, maxPitch);
        ObjectsSFX.PlayOneShot(clip);
    }
    public void PlayRobotSFX(AudioClip clip)
    {
        RobotMovementSFX.pitch = Random.Range(minPitch, maxPitch);
        RobotMovementSFX.PlayOneShot(clip);
    }
    public void PlayFootstepSounds()
    {
        // Check if the player is walking
        if (isWalking)
        {
            // Check if enough time has passed to play the next footstep sound
            if (Time.time - timeSinceLastFootstep >= Random.Range(minTimeBetweenFootsteps, maxTimeBetweenFootsteps))
            {
                // Play a random footstep sound from the array
                AudioClip footstepSound = footstepSounds[Random.Range(0, footstepSounds.Length)];
                RobotMovementSFX.pitch = Random.Range(minPitch, maxPitch); // Set pitch variation
                RobotMovementSFX.PlayOneShot(footstepSound);

                timeSinceLastFootstep = Time.time; // Update the time since the last footstep sound
            }
        }
    }

    public void StartWalkingSound()
    {
        isWalking = true;
    }

    public void StopWalkingSound()
    {
        isWalking = false;
    }

    public void StartLandingSound()
    {
        PlayRobotSFX(landing);
    }

    public void StartJumpingSound()
    {
        PlayRobotSFX(Jumping);
    }


}
