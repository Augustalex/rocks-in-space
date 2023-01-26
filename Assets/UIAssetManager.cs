using System;
using UnityEngine;

public class UIAssetManager : MonoBehaviour
{
    public Texture upIcon;
    public Texture doubleUpIcon;
    public Texture downIcon;
    public Texture doubleDownIcon;

    public Texture oreIcon;
    public Texture ironIcon;
    public Texture graphiteIcon;
    public Texture copperIcon;
    public Texture metalsIcon;
    public Texture gadgetsIcon;
    public Texture powerIcon;
    public Texture iceIcon;
    public Texture waterIcon;
    public Texture refreshmentsIcon;
    public Texture proteinIcon;
    public Texture foodIcon;
    public Texture housingIcon;
    public Texture creditsIcon;
    public Texture workersIcon;

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
            TinyPlanetResources.ResourceTrend.DoubleDown => doubleDownIcon,
            TinyPlanetResources.ResourceTrend.Down => downIcon,
            TinyPlanetResources.ResourceTrend.Up => upIcon,
            TinyPlanetResources.ResourceTrend.DoubleUp => doubleUpIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(trend), trend, null)
        };
    }

    public Texture GetResourceTexture(TinyPlanetResources.PlanetResourceType resourceType)
    {
        return resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Ore => oreIcon,
            TinyPlanetResources.PlanetResourceType.Iron => ironIcon,
            TinyPlanetResources.PlanetResourceType.Graphite => graphiteIcon,
            TinyPlanetResources.PlanetResourceType.Copper => copperIcon,
            TinyPlanetResources.PlanetResourceType.Metals => metalsIcon,
            TinyPlanetResources.PlanetResourceType.Gadgets => gadgetsIcon,
            TinyPlanetResources.PlanetResourceType.Ice => iceIcon,
            TinyPlanetResources.PlanetResourceType.Water => waterIcon,
            TinyPlanetResources.PlanetResourceType.Refreshments => refreshmentsIcon,
            TinyPlanetResources.PlanetResourceType.Energy => powerIcon,
            TinyPlanetResources.PlanetResourceType.Protein => proteinIcon,
            TinyPlanetResources.PlanetResourceType.Food => foodIcon,
            TinyPlanetResources.PlanetResourceType.Housing => housingIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
        };
    }
}