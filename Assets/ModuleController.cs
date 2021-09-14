using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private double _rate = .5f;
    private double _cooldown = 0;
    private int _life = 100;
    private PowerControlled _powerControlled;
    private bool _dead;

    void Start()
    {
        _powerControlled = GetComponentInChildren<PowerControlled>();
        _planetResources = GetComponentInParent<TinyPlanetResources>();

        _planetResources.SetInhabitants(_planetResources.GetInhabitants() + 100);
    }

    void Update()
    {
        if (_dead) return;
        
        if (_life <= 0)
        {
            _dead = true;
            _powerControlled.PowerOff();
            _planetResources.SetInhabitants(_planetResources.GetInhabitants() - 100);
        }

        if (_cooldown >= 1f)
        {
            _cooldown = 0;

            var food = _planetResources.GetFood();
            if (food >= 10)
            {
                _planetResources.SetFood(food - 10);
            }
            else
            {
                _life -= 10;
            }
        }
        else
        {
            _cooldown += _rate * Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        if (!_dead)
        {
            _planetResources.SetInhabitants(_planetResources.GetInhabitants() - 100);
        }
    }
}
