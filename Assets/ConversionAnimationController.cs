using System;
using UnityEngine;

[RequireComponent(typeof(ResourceConversionEffect))]
public class ConversionAnimationController : MonoBehaviour
{
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        var conversionEffect = GetComponent<ResourceConversionEffect>();
        conversionEffect.OnStarted += StartAnimation;
        conversionEffect.OnStopped += StopAnimation;
    }

    private void StopAnimation()
    {
        _animator.enabled = false;
    }

    private void StartAnimation()
    {
        _animator.enabled = true;
    }
}