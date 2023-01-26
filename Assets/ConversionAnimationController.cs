using System;
using UnityEngine;

[RequireComponent(typeof(ResourceConversionEffect))]
public class ConversionAnimationController : MonoBehaviour
{
    private Animator _animator;
    private float _originalSpeed;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        var conversionEffect = GetComponent<ResourceConversionEffect>();
        conversionEffect.OnStarted += StartAnimation;
        conversionEffect.OnStopped += StopAnimation;
        conversionEffect.OnSlowedDown += SlowedDown;
        conversionEffect.OnResumedSpeed += ResumedSpeed;
    }

    private void Start()
    {
        StopAnimation();
    }

    private void StopAnimation()
    {
        _animator.enabled = false;
    }

    private void StartAnimation()
    {
        _animator.enabled = true;
    }

    private void SlowedDown()
    {
        _originalSpeed = _animator.speed;
        _animator.speed = _originalSpeed * .5f;
    }

    private void ResumedSpeed()
    {
        _animator.speed = _originalSpeed;
    }
}