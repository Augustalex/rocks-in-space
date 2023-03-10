using System;
using GameNotifications;
using UnityEngine;

public class NotificationSounds : MonoBehaviour
{
    public AudioClip informative;
    public float informativeVolume;
    public AudioClip alert;
    public float alertVolume;
    public AudioClip negative;
    public float negativeVolume;
    public AudioClip positive;
    public float positiveVolume;

    private static NotificationSounds _instance;
    private AudioSource _audioSource;

    private Tuple<float, NotificationTypes> _lastPlayed = null;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static NotificationSounds Get()
    {
        return _instance;
    }

    public void Play(NotificationTypes notificationType)
    {
        if (_lastPlayed != null && _lastPlayed.Item2 == notificationType && Time.time - _lastPlayed.Item1 < 4f) return;

        var clip = notificationType switch
        {
            NotificationTypes.Informative => informative,
            NotificationTypes.Alerting => alert,
            NotificationTypes.Positive => positive,
            NotificationTypes.Negative => negative,
            _ => informative
        };
        _audioSource.clip = clip;

        var volume = notificationType switch
        {
            NotificationTypes.Informative => informativeVolume,
            NotificationTypes.Alerting => alertVolume,
            NotificationTypes.Positive => positiveVolume,
            NotificationTypes.Negative => negativeVolume,
            _ => 0f
        };
        _audioSource.PlayOneShot(clip, volume);

        _lastPlayed = new Tuple<float, NotificationTypes>(Time.time, notificationType);
        // _audioSource.pitch = Random.Range(.98f, 1.02f);
        // _audioSource.Play();
    }
}