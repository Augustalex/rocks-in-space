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

    public void Play(NotificationTypes notificationTypes)
    {
        Debug.Log("PLAY NOTIFICATION SOUND: " + notificationTypes);
        // _audioSource.Stop();

        var clip = notificationTypes switch
        {
            NotificationTypes.Informative => informative,
            NotificationTypes.Alerting => alert,
            NotificationTypes.Positive => positive,
            NotificationTypes.Negative => negative,
        };
        _audioSource.clip = clip;

        var volume = notificationTypes switch
        {
            NotificationTypes.Informative => informativeVolume,
            NotificationTypes.Alerting => alertVolume,
            NotificationTypes.Positive => positiveVolume,
            NotificationTypes.Negative => negativeVolume,
        };
        _audioSource.PlayOneShot(clip, volume);

        // _audioSource.pitch = Random.Range(.98f, 1.02f);
        // _audioSource.Play();
    }
}