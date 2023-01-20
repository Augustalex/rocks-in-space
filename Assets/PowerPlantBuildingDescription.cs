using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BuildingDescription))]
[RequireComponent(typeof(ResourceEffect))]
public class PowerPlantBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var effect = GetComponent<ResourceEffect>();
        return
            $"Provides {effect.energy} {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)}, required by farms and residencies.";
    }
}