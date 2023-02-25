using System;
using System.Collections.Generic;
using GameNotifications;
using Interactors;
using UnityEngine;

public class PlanetProductionMonitor : MonoBehaviour
{
    private readonly List<ProductionStatus> _productionStatuses = new();

    private TinyPlanet _planet;

    private readonly Dictionary<BuildingType, int> _stoppedByType = new();

    private void Awake()
    {
        _planet = GetComponent<TinyPlanet>();

        _productionStatuses.AddRange(new[]
        {
            new ProductionStatus(_planet, BuildingType.Refinery),
            new ProductionStatus(_planet, BuildingType.CopperRefinery),
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
        if (!_stoppedByType.ContainsKey(buildingType)) _stoppedByType[buildingType] = 0;
        _stoppedByType[buildingType] -= 1;
    }

    public void RegisterProductionStop(BuildingType buildingType, bool silently)
    {
        if (!_stoppedByType.ContainsKey(buildingType)) _stoppedByType[buildingType] = 0;
        _stoppedByType[buildingType] += 1;

        if (!silently)
        {
            var productionStatus = _productionStatuses.Find(p => p.Is(buildingType));
            productionStatus?.Stopped();
        }
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

    public void BuildingWasDetached(BuildingType buildingType, bool isRunning)
    {
        if (!isRunning && _stoppedByType.ContainsKey(buildingType)) _stoppedByType[buildingType] -= 1;
    }

    public void BuildingWasAttached(BuildingType buildingType, bool isRunning)
    {
        if (!isRunning)
        {
            if (!_stoppedByType.ContainsKey(buildingType)) _stoppedByType[buildingType] = 0;

            _stoppedByType[buildingType] += 1;
        }
    }

    public int GetStoppedBuildingsCount(BuildingType buildingType)
    {
        if (!_stoppedByType.ContainsKey(buildingType)) return 0;
        return _stoppedByType[buildingType];
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
            BuildingType.Refinery => "Iron refineries",
            BuildingType.CopperRefinery => "Copper refineries",
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
            BuildingType.Refinery => "Iron refineries",
            BuildingType.CopperRefinery => "Copper refineries",
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
            Message = $"{message} at {_planet.planetName} are lacking workers and is running at a reduced speed."
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