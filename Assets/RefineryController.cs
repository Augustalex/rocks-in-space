using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefineryController : MonoBehaviour
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

            var ore = _planetResources.GetOre();
            if (ore >= 10)
            {
                _planetResources.SetOre(ore - 10);
                _planetResources.SetMetals(_planetResources.GetMetals() + 1);
            }
        }
        else
        {
            _cooldown += _rate * Time.deltaTime;
        }
    }
}
