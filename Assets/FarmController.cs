using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private double _rate = .1f;
    private double _cooldown = 0;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();

        _planetResources.SetEnergy(_planetResources.GetEnergy() - 50);
    }

    void Update()
    {
        if (_cooldown >= 1f)
        {
            _cooldown = 0;

            var energy = _planetResources.GetEnergy();
            if (energy >= 0)
            {
                _planetResources.SetFood(_planetResources.GetFood() + 100);
            }
        }
        else
        {
            _cooldown += _rate * Time.deltaTime;
        }
    }
}
