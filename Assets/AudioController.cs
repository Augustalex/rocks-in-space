using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip destroyBlock;
    public float destroyBlockVolume;

    private static AudioController _instance;
    private Camera _camera;
    private AudioSource _audioSource;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _camera = Camera.main;
        _audioSource = _camera.GetComponent<AudioSource>();
    }

    public static AudioController Get()
    {
        return _instance;
    }

    public void Play(AudioClip clip, float volume, Vector3 position)
    {
        _audioSource.PlayOneShot(clip, volume);
        // AudioSource.PlayClipAtPoint(clip, position, volume);
    }
}
