using UnityEngine;

[RequireComponent(typeof(ResourceConversionEffect))]
[RequireComponent(typeof(BuildingDescription))]
public class ConversionBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var conversion = GetComponent<ResourceConversionEffect>();
        return
            $"Converts {TinyPlanetResources.ResourceName(conversion.from)} into {TinyPlanetResources.ResourceName(conversion.to)} every {conversion.iterationTime} seconds";
    }
}