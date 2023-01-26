using UnityEngine;

[RequireComponent(typeof(BuildingDescription))]
[RequireComponent(typeof(ResourceEffect))]
public class PowerPlantBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var effect = GetComponent<ResourceEffect>();
        return
            $"Takes: 1 {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Water)} every 2 seconds\nProvides: {effect.energy} {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)}\n";
    }
}