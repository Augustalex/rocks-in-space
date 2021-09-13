using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private double _rate = .5f;
    private double _cooldown = 0;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
    }

    void Update()
    {
        if (_cooldown >= 1f)
        {
            _cooldown = 0;
            
            if (_planetResources.metals >= 10)
            {
                _planetResources.metals -= 10;
                _planetResources.gadgets += 10;
            }
        }
        else
        {
            _cooldown += _rate * Time.deltaTime;
        }
    }
}
