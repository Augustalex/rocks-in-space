using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private float _pauseUntil = 0;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        RockSmash.Get().OnExplosion += OnExplosion;
    }

    private void OnExplosion()
    {
        _pauseUntil = Time.time + 30;
    }

    private void Update()
    {
        if (_pauseUntil < 0) return;
        if (Time.time < _pauseUntil)
        {
            if (_audioSource.isPlaying) _audioSource.Stop();
        }
        else if (Time.time >= _pauseUntil)
        {
            if (!_audioSource.isPlaying) _audioSource.Play();
        }
    }
}