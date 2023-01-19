using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSounds : MonoBehaviour
{
    public AudioClip chime1;
    public AudioClip chime2;
    public float chimeVolume;
    
    public AudioClip clink1;
    public float clinkVolume;

    private static ResourceSounds _instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static ResourceSounds Get()
    {
        return _instance;
    }

    public void Cancel()
    {
        _audioSource.Stop();
    }

    public void Play()
    {
        _audioSource.Stop();
        _audioSource.clip = Random.value < .5f ? chime1 : chime2;
        _audioSource.volume = chimeVolume;
        _audioSource.pitch = Random.Range(.5f, 1.5f);
        _audioSource.Play();
    }
    
    public void PlayClink()
    {
        _audioSource.Stop();
        _audioSource.clip = clink1;
        _audioSource.volume = clinkVolume;
        _audioSource.pitch = Random.Range(.5f, 1.5f);
        _audioSource.Play();
    }
}