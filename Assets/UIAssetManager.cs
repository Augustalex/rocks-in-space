using System;
using UnityEngine;

public class UIAssetManager : MonoBehaviour
{
    public Texture upIcon;
    public Texture doubleUpIcon;
    public Texture downIcon;
    public Texture doubleDownIcon;

    public Texture oreIcon;
    public Texture metalsIcon;
    public Texture gadgetsIcon;
    public Texture powerIcon;
    public Texture foodIcon;
    public Texture housingIcon;
    public Texture creditsIcon;

    private static UIAssetManager _instance;

    public static UIAssetManager Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    public Texture GetTrendTexture(TinyPlanetResources.ResourceTrend trend)
    {
        return trend switch
        {
            TinyPlanetResources.ResourceTrend.doubleDown => doubleDownIcon,
            TinyPlanetResources.ResourceTrend.down => downIcon,
            TinyPlanetResources.ResourceTrend.up => upIcon,
            TinyPlanetResources.ResourceTrend.doubleUp => doubleUpIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(trend), trend, null)
        };
    }

    public Texture GetResourceTexture(TinyPlanetResources.PlanetResourceType resourceType)
    {
        return resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Ore => oreIcon,
            TinyPlanetResources.PlanetResourceType.Metals => metalsIcon,
            TinyPlanetResources.PlanetResourceType.Gadgets => gadgetsIcon,
            TinyPlanetResources.PlanetResourceType.Energy => powerIcon,
            TinyPlanetResources.PlanetResourceType.Food => foodIcon,
            TinyPlanetResources.PlanetResourceType.Housing => housingIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
        };
    }
}