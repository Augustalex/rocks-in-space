using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Route
{
    public readonly PlanetId StartPlanetId;
    public readonly PlanetId DestinationPlanetId;
    public TinyPlanetResources.PlanetResourceType ResourceType = TinyPlanetResources.PlanetResourceType.Ore;

    public event Action Removed;

    private float _orePerSecond = 0;
    private float _metalsPerSecond = 0;
    private float _gadgetsPerSecond = 0;

    private readonly Queue<float> _transfers = new();

    private readonly int _order;
    private bool _isActive;
    private bool _removed;

    public Route(PlanetId start, PlanetId destination, int order)
    {
        _order = order;
        StartPlanetId = start;
        DestinationPlanetId = destination;
    }

    public void Run()
    {
        if (_removed) return;

        var start = PlanetsRegistry.Get().FindPlanetById(StartPlanetId);
        if (start == null || !start.HasPort()) return;

        var destination = PlanetsRegistry.Get().FindPlanetById(DestinationPlanetId);
        if (destination == null || !destination.HasPort()) return;

        var startingResources = start.GetResources();
        var destinationResources = destination.GetResources();

        if (_orePerSecond > 0)
        {
            var preferredTake = _orePerSecond * Time.deltaTime;
            var toTake = Mathf.Min(startingResources.GetOre(), preferredTake);

            startingResources.RemoveOre(toTake);
            destinationResources.AddOre(toTake);

            if (_transfers.Count >= InactiveRouteFramesThreshold()) _transfers.Dequeue();
            _transfers.Enqueue(toTake);
        }

        if (_metalsPerSecond > 0)
        {
            var preferredTake = _metalsPerSecond * Time.deltaTime;
            var toTake = Mathf.Min(startingResources.GetMetals(), preferredTake);

            startingResources.RemoveMetals(toTake);
            destinationResources.AddMetals(toTake);

            if (_transfers.Count >= InactiveRouteFramesThreshold()) _transfers.Dequeue();
            _transfers.Enqueue(toTake);
        }

        if (_gadgetsPerSecond > 0)
        {
            var preferredTake = _gadgetsPerSecond * Time.deltaTime;
            var toTake = Mathf.Min(startingResources.GetGadgets(), preferredTake);

            startingResources.RemoveGadgets(toTake);
            destinationResources.AddGadgets(toTake);

            if (_transfers.Count >= InactiveRouteFramesThreshold()) _transfers.Dequeue();
            _transfers.Enqueue(toTake);
        }

        _isActive = _transfers.Count >= InactiveRouteFramesThreshold() && _transfers.Sum() > 0f;
    }

    public bool Is(TinyPlanet start, TinyPlanet end)
    {
        return start.planetId.Is(StartPlanetId) && end.planetId.Is(DestinationPlanetId);
    }

    public void SetTrade(TinyPlanetResources.PlanetResourceType planetResourceType, float amountPerSecond)
    {
        if (planetResourceType == TinyPlanetResources.PlanetResourceType.Ore)
        {
            ResourceType = planetResourceType;
            _orePerSecond = amountPerSecond;

            _metalsPerSecond = 0;
            _gadgetsPerSecond = 0;
        }
        else if (planetResourceType == TinyPlanetResources.PlanetResourceType.Metals)
        {
            ResourceType = planetResourceType;
            _metalsPerSecond = amountPerSecond;

            _orePerSecond = 0;
            _gadgetsPerSecond = 0;
        }
        else if (planetResourceType == TinyPlanetResources.PlanetResourceType.Gadgets)
        {
            ResourceType = planetResourceType;
            _gadgetsPerSecond = amountPerSecond;

            _orePerSecond = 0;
            _metalsPerSecond = 0;
        }
    }

    public bool StartsFrom(TinyPlanet planet)
    {
        return StartPlanetId.Is(planet.planetId);
    }

    public bool FromTo(TinyPlanet start, TinyPlanet end)
    {
        return start.planetId.Is(StartPlanetId) && end.planetId.Is(DestinationPlanetId);
    }

    public int Order()
    {
        return _order;
    }

    public bool IsActive()
    {
        return _isActive;
    }

    public void Remove()
    {
        _removed = true;
        Removed?.Invoke();
    }

    private float InactiveRouteFramesThreshold()
    {
        return SettingsManager.Get().miscSettings.inactiveRouteFramesThreshold;
    }
}