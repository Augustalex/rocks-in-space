using System;
using System.Collections.Generic;
using System.Linq;
using Interactors;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private static ProgressManager _instance;

    private readonly HashSet<BuildingType> _builtBuildings = new();
    private readonly HashSet<PlanetId> _ports = new();
    private int _neutralColonists;
    private int _happyColonists;
    private int _overjoyedColonists;

    public static ProgressManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
    }

    public void Built(BuildingType buildingType)
    {
        _builtBuildings.Add(buildingType);
    }

    public void BuiltPort(PlanetId planetId)
    {
        _ports.Add(planetId);
    }

    public void DestroyedPort(PlanetId planetId)
    {
        _ports.Remove(planetId);
    }

    public bool CanTrade()
    {
        return _ports.Count >= 2;
    }

    public bool HasBuilt(BuildingType buildingType)
    {
        return _builtBuildings.Contains(buildingType);
    }

    public int TotalColonistsCount()
    {
        return PlanetsRegistry.Get().All().Sum(planet => planet.GetResources().GetInhabitants());
    }

    public void UpdateProgress()
    {
        _neutralColonists = 0;
        _happyColonists = 0;
        _overjoyedColonists = 0;

        foreach (var tinyPlanet in PlanetsRegistry.Get().All())
        {
            var colonistsMonitor = tinyPlanet.GetColonistMonitor();
            var status = colonistsMonitor.GetPlanetStatus();
            var colonists = tinyPlanet.GetResources().GetInhabitants();

            if (status == PlanetColonistMonitor.PlanetStatus.Neutral)
            {
                _neutralColonists += colonists;
            }
            else if (status == PlanetColonistMonitor.PlanetStatus.Happy)
            {
                _happyColonists += colonists;
            }
            else if (status == PlanetColonistMonitor.PlanetStatus.Overjoyed)
            {
                _overjoyedColonists += colonists;
            }
        }
    }

    public int GetColonistCount(PlanetColonistMonitor.PlanetStatus status)
    {
        return status switch
        {
            PlanetColonistMonitor.PlanetStatus.Uninhabited => 0,
            PlanetColonistMonitor.PlanetStatus.MovingOut => 0,
            PlanetColonistMonitor.PlanetStatus.Neutral => _neutralColonists,
            PlanetColonistMonitor.PlanetStatus.Happy => _happyColonists,
            PlanetColonistMonitor.PlanetStatus.Overjoyed => _overjoyedColonists,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}