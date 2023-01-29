using System;
using System.Collections;
using System.Collections.Generic;
using Interactors;
using UnityEngine;

public class ProductionInfoItemRow : MonoBehaviour
{
    public GameObject productionInfoItemTemplate;

    public BuildingType[] buildingTypes;

    private readonly Dictionary<BuildingType, GameObject> _itemByBuilding = new();

    void Awake()
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
        }
    }

    private void Start()
    {
        ProgressManager.Get();

        foreach (var (buildingType, item) in _itemByBuilding)
        {
            item.SetActive(false);
        }

        StartCoroutine(SlowUpdate());
    }

    IEnumerator SlowUpdate()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(2f);
            UpdateNow();
        }
    }

    private void UpdateNow()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (!currentPlanet) return;

        var resources = currentPlanet.GetResources();

        foreach (var (buildingType, item) in _itemByBuilding)
        {
            item.SetActive(resources.HasBuilding(buildingType));
        }
    }
}