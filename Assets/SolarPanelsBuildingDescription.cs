using UnityEngine;

public class SolarPanelsBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var effect = GetComponent<ResourceEffect>();
        return
            $"Provides {effect.energy} {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)}.";
    }
}