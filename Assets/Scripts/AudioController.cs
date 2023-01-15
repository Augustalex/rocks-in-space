using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip laserStarted;
    public float laserStartedVolume;

    public AudioClip laserProgress;
    public float laserProgressVolume;

    public AudioClip laserFinish;
    public float laserFinishVolume;

    public AudioClip build;
    public float buildVolume;

    public AudioClip cannotBuild;
    public float cannotBuildVolume;

    private static AudioController _instance;
    private Camera _camera;
    private AudioSource _audioSource;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _camera = GetComponentInParent<Camera>();
        _audioSource = _camera.GetComponent<AudioSource>();
    }

    public static AudioController Get()
    {
        return _instance;
    }

    public void Cancel()
    {
        _audioSource.Stop();
    }

    public void PlayWithRandomPitch(AudioClip clip, float volume, float delay = 0)
    {
        _audioSource.Stop();
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = Random.Range(.65f, 1.2f);
        _audioSource.PlayDelayed(delay);
    }

    public void Play(AudioClip clip, float volume, Vector3 position)
    {
        var cameraPosition = transform.position;
        var direction = (position - cameraPosition).normalized;
        var audioPosition = cameraPosition + direction * 1f;
        AudioSource.PlayClipAtPoint(clip, audioPosition, volume);
    }
}