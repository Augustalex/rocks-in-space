using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ShakeProfile
{
    public float targetThreshold;
    public float shakeSpeed;
    public float shakeAmount;
}

public class CameraShake : MonoBehaviour
{
    public float shortShakeTime = .25f;

    [SerializeField] public ShakeProfile shortShake;

    [SerializeField] public ShakeProfile continuousShake;

    private Vector3 _originalPos;
    private static CameraShake _instance;
    private float _waitUntil;
    private float _shakeUntil;
    private bool _shakeFor;
    private float _shakeStarted;
    private Vector3 _targetPosition;
    private bool _shakeContinuous;

    private void Awake()
    {
        _instance = this;
        _originalPos = transform.localPosition;
    }

    void Update()
    {
        var cameraTransform = transform;

        if (_shakeFor)
        {
            if (Time.time < _shakeUntil)
            {
                var distance = Vector3.Distance(cameraTransform.localPosition, _targetPosition);
                var inDistance = distance < shortShake.shakeAmount * shortShake.targetThreshold;
                if (inDistance)
                {
                    _targetPosition = Random.insideUnitSphere * shortShake.shakeAmount;
                }
                else
                {
                    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, _targetPosition,
                        Time.deltaTime * shortShake.shakeSpeed);
                }
            }
            else
            {
                cameraTransform.localPosition = _originalPos;
                _shakeFor = false;
            }
        }
        else if (_shakeContinuous)
        {
            var distance = Vector3.Distance(cameraTransform.localPosition, _targetPosition);
            var inDistance = distance < continuousShake.shakeAmount * continuousShake.targetThreshold;
            if (inDistance)
            {
                _targetPosition = Random.insideUnitSphere * continuousShake.shakeAmount;
            }
            else
            {
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, _targetPosition,
                    Time.deltaTime * continuousShake.shakeSpeed);
            }
        }
    }

    public static void Shake()
    {
        CameraShake.Get().ShakeNow();
    }

    public static void StopShaking()
    {
        CameraShake.Get().StopShakingNow();
    }

    public static void ShortShake()
    {
        CameraShake.Get().StartShortShake();
    }

    private static CameraShake Get()
    {
        return _instance;
    }

    private void StartShortShake()
    {
        _shakeFor = true;
        _shakeUntil = Time.time + shortShakeTime;

        transform.localPosition = _originalPos;
        _targetPosition = transform.localPosition;

        // _shakeStarted = Time.time;
    }

    public void ShakeNow()
    {
        _shakeFor = false;
        _shakeContinuous = true;

        transform.localPosition = _originalPos;
        _targetPosition = transform.localPosition;
    }

    private void StopShakingNow()
    {
        _shakeFor = false;
        _shakeContinuous = false;

        transform.localPosition = _originalPos;
        _targetPosition = transform.localPosition;
    }

    private float CalcShake(float shakeDamper, float shakeTime, AnimationCurve curve)
    {
        return Mathf.PerlinNoise(shakeTime / shakeDamper, 0f) * curve.Evaluate(shakeTime);
    }
}