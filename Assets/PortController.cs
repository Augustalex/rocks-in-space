using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private double _rate = .1f;
    private double _cooldown = 0;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
    }

    void Update()
    {
    }
}
