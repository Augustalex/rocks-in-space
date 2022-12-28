using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEffect : MonoBehaviour
{
    private bool _activated;
    private Vector3 _currentTarget;
    private Camera _camera;
    private ParticleSystem[] _particleSystems;
    private GameObject _light;

    private void Awake()
    {
        _particleSystems = GetComponentsInChildren<ParticleSystem>();
        _light = GetComponentInChildren<Light>().gameObject;
    }

    void Start()
    {
        _camera = GetComponentInParent<Camera>();

        Stop();
    }

    public void SetTarget(Vector3 target)
    {
        if (target != _currentTarget)
        {
            _currentTarget = target;
            
            transform.position = _camera.transform.position;
            transform.LookAt(_currentTarget);
            transform.position = _currentTarget;
        }
    }
    
    public void Activate()
    {
        CameraShake.Shake();
        
        _activated = true;
        foreach (var system in _particleSystems)
        {
            system.Play();
        }
        _light.SetActive(true);
    }

    public void Stop()
    {
        CameraShake.StopShaking();
        _activated = false;
        foreach (var system in _particleSystems)
        {
            system.Stop();
        }
        _light.SetActive(false);
    }

    public bool IsActivated()
    {
        return _activated;
    }
}
