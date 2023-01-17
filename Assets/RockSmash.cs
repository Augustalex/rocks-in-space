using UnityEngine;

public class RockSmash : MonoBehaviour
{
    public AudioClip melt;
    public float meltVolume;

    public AudioClip smash;
    public float smashVolume;

    public AudioClip hit;
    public float hitVolume;

    private static RockSmash _instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public static RockSmash Get()
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
        _audioSource.clip = smash;
        _audioSource.volume = smashVolume;
        _audioSource.pitch = Random.Range(.5f, 1.5f);
        _audioSource.Play();
    }

    public void PlayMelt()
    {
        _audioSource.Stop();
        _audioSource.clip = melt;
        _audioSource.volume = meltVolume;
        _audioSource.pitch = Random.Range(.8f, 1.3f);
        _audioSource.Play();
    }

    public void PlayHitAndSmash(Vector3 target)
    {
        _audioSource.Stop();
        _audioSource.clip = smash;
        _audioSource.volume = smashVolume;
        _audioSource.pitch = Random.Range(.5f, 1.5f);
        _audioSource.Play();

        var cameraPosition = transform.position;
        var direction = (target - cameraPosition).normalized;
        var audioPosition = cameraPosition + direction * 1f;
        AudioSource.PlayClipAtPoint(hit, audioPosition, hitVolume);
    }
}