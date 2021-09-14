using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPlantController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
        _planetResources.SetEnergy(_planetResources.GetEnergy() + 100);
    }
}
