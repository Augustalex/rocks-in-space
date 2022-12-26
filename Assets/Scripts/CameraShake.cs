using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    // Transform of the camera to shake. Grabs the gameObject's transform
    // if null.

    // How long the object should shake for.
    private bool _shaking = false;
	
    // Amplitude of the shake. A larger value shakes the camera harder.
    private const float ShakeAmount = 0.015f;

    private Vector3 _originalPos;
    private static CameraShake _instance;

    private void Awake()
    {
        _instance = this;
        _originalPos = transform.localPosition;
    }

    void Update()
    {
        var camTransform1 = transform;
        if (_shaking)
        {
            camTransform1.localPosition = _originalPos + Random.insideUnitSphere * ShakeAmount;
        }
        else
        {
            camTransform1.localPosition = _originalPos;
        }
    }

    public static void Shake()
    {
        CameraShake.Get().ShakeNow();
    }

    private static CameraShake Get()
    {
        return _instance;
    }

    public void ShakeNow()
    {
        _shaking = true;
    }

    public static void StopShaking()
    {
        CameraShake.Get().StopShakingNow();
    }

    private void StopShakingNow()
    {
        _shaking = false;
    }
}