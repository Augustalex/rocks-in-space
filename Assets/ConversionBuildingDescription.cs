using UnityEngine;

[RequireComponent(typeof(ResourceConversionEffect))]
[RequireComponent(typeof(BuildingDescription))]
public class ConversionBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var conversion = GetComponent<ResourceConversionEffect>();
        if (conversion.fromSecondaryAmount > 0)
        {
            return
                $"Takes: {conversion.fromAmount} {TinyPlanetResources.ResourceName(conversion.from)} + {conversion.fromSecondaryAmount} {TinyPlanetResources.ResourceName(conversion.fromSecondary)}\n Produce: {conversion.toAmount} {TinyPlanetResources.ResourceName(conversion.to)}\nevery {conversion.iterationTime} seconds";
        }

        return
            $"Takes: {conversion.fromAmount} {TinyPlanetResources.ResourceName(conversion.from)}\nProduces: {conversion.toAmount} {TinyPlanetResources.ResourceName(conversion.to)}\nevery {conversion.iterationTime} seconds";
    }
}