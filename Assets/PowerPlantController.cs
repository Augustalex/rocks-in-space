using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlantController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private const float PowerAdd = 100f;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
        _planetResources.AddEnergy(PowerAdd);
    }
}
