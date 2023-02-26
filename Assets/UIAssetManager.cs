using System;
using Interactors;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    [FormerlySerializedAs("metalsIcon")] public Texture ironPlatesIcon;
    public Texture copperPlatesIcon;
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

    [Header("Building icons")] public Texture refineryIcon;
    public Texture copperRefineryIcon;
    public Texture factoryIcon;
    public Texture solarPanelsIcon;
    public Texture proteinFabricatorIcon;
    public Texture housesIcon;
    public Texture purifierIcon;
    public Texture distilleryIcon;
    public Texture farmsIcon;
    public Texture powerPlantIcon;

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
            TinyPlanetResources.PlanetResourceType.IronOre => ironIcon,
            TinyPlanetResources.PlanetResourceType.Graphite => graphiteIcon,
            TinyPlanetResources.PlanetResourceType.CopperOre => copperIcon,
            TinyPlanetResources.PlanetResourceType.IronPlates => ironPlatesIcon,
            TinyPlanetResources.PlanetResourceType.CopperPlates => copperPlatesIcon,
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

    public Texture GetBuildingIcon(BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Refinery => refineryIcon,
            BuildingType.CopperRefinery => copperRefineryIcon,
            BuildingType.Factory => factoryIcon,
            BuildingType.PowerPlant => powerPlantIcon,
            BuildingType.FarmDome => farmsIcon,
            BuildingType.ResidentModule => housesIcon,
            BuildingType.Purifier => purifierIcon,
            BuildingType.Distillery => distilleryIcon,
            BuildingType.SolarPanels => solarPanelsIcon,
            BuildingType.ProteinFabricator => proteinFabricatorIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
        };
    }
}