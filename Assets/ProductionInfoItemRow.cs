using System;
using System.Collections.Generic;
using Interactors;
using UnityEngine;

public class ProductionInfoItemRow : MonoBehaviour
{
    public GameObject productionInfoItemTemplate;

    public BuildingType[] buildingTypes;

    private readonly Dictionary<BuildingType, GameObject> _itemByBuilding = new();

    public void Setup()
    {
        foreach (var buildingType in buildingTypes)
        {
            var itemObject = Instantiate(productionInfoItemTemplate, transform);
            var infoItem = itemObject.GetComponent<ProductionInfoItem>();
            infoItem.buildingType = buildingType;
            infoItem.resourceType = buildingType switch
            {
                BuildingType.Refinery => TinyPlanetResources.PlanetResourceType.Metals,
                BuildingType.Factory => TinyPlanetResources.PlanetResourceType.Gadgets,
                BuildingType.PowerPlant => TinyPlanetResources.PlanetResourceType.Energy,
                BuildingType.FarmDome => TinyPlanetResources.PlanetResourceType.Food,
                BuildingType.ResidentModule => TinyPlanetResources.PlanetResourceType.Housing,
                BuildingType.Purifier => TinyPlanetResources.PlanetResourceType.Water,
                BuildingType.Distillery => TinyPlanetResources.PlanetResourceType.Refreshments,
                BuildingType.SolarPanels => TinyPlanetResources.PlanetResourceType.Energy,
                BuildingType.ProteinFabricator => TinyPlanetResources.PlanetResourceType.Protein,
                _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
            };
            _itemByBuilding[buildingType] = itemObject;

            itemObject.SetActive(false);
        }
    }

    public void UpdateNow()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (!currentPlanet) return;

        var resources = currentPlanet.GetResources();

        foreach (var (buildingType, item) in _itemByBuilding)
        {
            item.SetActive(resources.HasBuilding(buildingType));
        }
    }

    public bool Empty()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (!currentPlanet) return true;

        foreach (var (buildingType, item) in _itemByBuilding)
        {
            if (currentPlanet.GetResources().HasBuilding(buildingType)) return false;
        }

        return true;
    }
}