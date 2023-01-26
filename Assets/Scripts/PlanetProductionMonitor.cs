using System;
using System.Collections.Generic;
using GameNotifications;
using Interactors;
using UnityEngine;

public class PlanetProductionMonitor : MonoBehaviour
{
    private readonly List<ProductionStatus> _productionStatuses = new();

    private TinyPlanet _planet;

    private void Awake()
    {
        _planet = GetComponent<TinyPlanet>();

        _productionStatuses.AddRange(new[]
        {
            new ProductionStatus(_planet, BuildingType.Refinery),
            new ProductionStatus(_planet, BuildingType.Factory),
            new ProductionStatus(_planet, BuildingType.ProteinFabricator),
            new ProductionStatus(_planet, BuildingType.Purifier),
            new ProductionStatus(_planet, BuildingType.Distillery),
            new ProductionStatus(_planet, BuildingType.PowerPlant),
            new ProductionStatus(_planet, BuildingType.FarmDome),
        });
    }

    public void RegisterProductionStart(BuildingType buildingType)
    {
        // There is nothing to do on Production start at the moment
    }

    public void RegisterProductionStop(BuildingType buildingType)
    {
        var productionStatus = _productionStatuses.Find(p => p.Is(buildingType));
        productionStatus?.Stopped();
    }

    public void RegisterProductionResumeSpeed(BuildingType buildingType)
    {
        // There is nothing to do on Production resume speed at the moment
    }

    public void RegisterProductionSlowDown(BuildingType buildingType)
    {
        var productionStatus = _productionStatuses.Find(p => p.Is(buildingType));
        productionStatus?.SlowedDown();
    }
}

public class ProductionStatus
{
    private readonly NotificationThrottler _stopThrottler = new();
    private readonly NotificationThrottler _speedThrottler = new();

    private readonly TinyPlanet _planet;
    private readonly BuildingType _buildingType;

    public ProductionStatus(TinyPlanet planet, BuildingType buildingType)
    {
        _planet = planet;
        _buildingType = buildingType;
    }

    public void Stopped()
    {
        var message = _buildingType switch
        {
            BuildingType.Refinery => "Metal refineries",
            BuildingType.Factory => "Factories",
            BuildingType.ProteinFabricator => "Protein fabricators",
            BuildingType.Purifier => "Purifiers",
            BuildingType.Distillery => "Distilleries",
            BuildingType.PowerPlant => "Power plants",
            BuildingType.FarmDome => "Farms",
            _ => throw new ArgumentOutOfRangeException(nameof(_buildingType), _buildingType,
                "Production status class has no specified notification message for this building type.")
        };

        _stopThrottler.SendIfCanPost(new PlanetNotification()
        {
            Location = _planet,
            Message = $"{message} at {_planet.planetName} has stopped working"
        });
    }

    public void SlowedDown()
    {
        var message = _buildingType switch
        {
            BuildingType.Refinery => "Metal refineries",
            BuildingType.Factory => "Factories",
            BuildingType.ProteinFabricator => "Protein fabricators",
            BuildingType.Purifier => "Purifiers",
            BuildingType.Distillery => "Distilleries",
            BuildingType.PowerPlant => "Power plants",
            BuildingType.FarmDome => "Farms",
            _ => throw new ArgumentOutOfRangeException(nameof(_buildingType), _buildingType,
                "Production status class has no specified notification message for this building type.")
        };

        _speedThrottler.SendIfCanPost(new PlanetNotification()
        {
            Location = _planet,
            Message = $"{message} at {_planet.planetName} are lacking workers and is working at a half speed."
        });
    }

    public void Started()
    {
    }

    public bool Is(BuildingType buildingType)
    {
        return buildingType == _buildingType;
    }
}