using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Route
{
    public readonly PlanetId StartPlanetId;
    public readonly PlanetId DestinationPlanetId;
    public TinyPlanetResources.PlanetResourceType ResourceType = TinyPlanetResources.PlanetResourceType.IronOre;

    public event Action Removed;

    private readonly int _order;
    private bool _removed;

    private Dictionary<TinyPlanetResources.PlanetResourceType, int> _shipment;

    private static readonly Dictionary<TinyPlanetResources.PlanetResourceType, float> ResourceTimePerUnit = new()
    {
        { TinyPlanetResources.PlanetResourceType.IronOre, 10f },
        { TinyPlanetResources.PlanetResourceType.CopperOre, 10f },
        { TinyPlanetResources.PlanetResourceType.Graphite, 5f },
        { TinyPlanetResources.PlanetResourceType.CopperPlates, 1f },
        { TinyPlanetResources.PlanetResourceType.IronPlates, 1f },
        { TinyPlanetResources.PlanetResourceType.Gadgets, 1f },
        { TinyPlanetResources.PlanetResourceType.Water, 1f },
        { TinyPlanetResources.PlanetResourceType.Refreshments, 1f }
    };

    private float _routeStartedAt;
    private Dictionary<TinyPlanetResources.PlanetResourceType, int> _loaded = new();

    private float
        _returnTripTimeLeft =
            -1f; // If travel time is 0 and is returning, that is when goods are loaded. So starting state should be that the shipment is returning, to start the route properly.

    private float _runTimeLeft = -1f;

    private enum ShipmentTarget
    {
        Unloading,
        Loading
    }

    private ShipmentTarget
        _currentShipmentTarget =
            ShipmentTarget.Loading; // Start from From because it should start the route by loading new goods.

    public Route(PlanetId start, PlanetId destination, int order)
    {
        _order = order;
        StartPlanetId = start;
        DestinationPlanetId = destination;
    }

    public void Run(float timeDelta)
    {
        if (_removed) return;

        var start = PlanetsRegistry.Get().FindPlanetById(StartPlanetId);
        if (start == null || !start.HasPort())
        {
            Abort();
            return;
        }

        var destination = PlanetsRegistry.Get().FindPlanetById(DestinationPlanetId);
        if (destination == null || !destination.HasPort())
        {
            Abort();
            return;
        }

        var startingResources = start.GetResources();
        var destinationResources = destination.GetResources();

        if (_currentShipmentTarget == ShipmentTarget.Loading)
        {
            if (_returnTripTimeLeft < 0f)
            {
                _loaded.Clear();

                foreach (var goods in _shipment)
                {
                    var preferredTake = goods.Value;
                    var toTake =
                        Mathf.FloorToInt(Mathf.Min(startingResources.GetResource(ResourceType), preferredTake));
                    startingResources.RemoveResource(goods.Key, toTake);
                    _loaded.Add(goods.Key, toTake);
                }

                _runTimeLeft = GetTotalLoadedTime(start, destination, _loaded);
                _currentShipmentTarget = ShipmentTarget.Unloading;
            }
            else
            {
                _returnTripTimeLeft -= timeDelta;
            }
        }
        else if (_currentShipmentTarget == ShipmentTarget.Unloading)
        {
            if (_runTimeLeft < 0f)
            {
                foreach (var goods in _loaded)
                {
                    destinationResources.AddResource(goods.Key, goods.Value);
                }

                _loaded.Clear();

                _returnTripTimeLeft = GetUnloadedTime(start, destination);
                _currentShipmentTarget = ShipmentTarget.Loading;
            }
            else
            {
                _runTimeLeft -= timeDelta;
            }
        }
    }

    private void Abort()
    {
        var start = PlanetsRegistry.Get().FindPlanetById(StartPlanetId);
        if (start == null) return; // If planet no longer exists, then we loose whatever we have in the cargo!

        var startingResources = start.GetResources();
        foreach (var goods in _loaded)
        {
            startingResources.AddResource(goods.Key, goods.Value);
        }

        ResetShipment();
    }

    private void ResetShipment()
    {
        _loaded.Clear();
        _returnTripTimeLeft = -1f;
        _runTimeLeft = -1f;
        _currentShipmentTarget = ShipmentTarget.Loading;
    }

    public bool Is(TinyPlanet start, TinyPlanet end)
    {
        return start.PlanetId.Is(StartPlanetId) && end.PlanetId.Is(DestinationPlanetId);
    }

    public void SetTrade(Dictionary<TinyPlanetResources.PlanetResourceType, int> shipment)
    {
        _shipment = shipment;
        ResetShipment();
    }

    public static float GetShipmentTime(Dictionary<TinyPlanetResources.PlanetResourceType, int> shipment)
    {
        return shipment.Aggregate(0f, (acc, v) => acc + ResourceTimePerUnit[v.Key] * v.Value);
    }

    public static float GetTotalLoadedTime(TinyPlanet startPlanet, TinyPlanet targetPlanet,
        Dictionary<TinyPlanetResources.PlanetResourceType, int> shipment)
    {
        return GetUnloadedTime(startPlanet, targetPlanet) + GetShipmentTime(shipment);
    }

    public static float GetUnloadedTime(TinyPlanet startPlanet, TinyPlanet targetPlanet)
    {
        var distance = startPlanet.GetDistanceTo(targetPlanet);
        var distanceSeconds = distance / 40f;
        return Mathf.FloorToInt(distanceSeconds);
    }

    public bool StartsFrom(TinyPlanet planet)
    {
        return StartPlanetId.Is(planet.PlanetId);
    }

    public bool FromTo(TinyPlanet start, TinyPlanet end)
    {
        return start.PlanetId.Is(StartPlanetId) && end.PlanetId.Is(DestinationPlanetId);
    }

    public int Order()
    {
        return _order;
    }

    public bool IsActive()
    {
        return true;
    }

    public void Remove()
    {
        Abort();
        _removed = true;
        Removed?.Invoke();
    }

    private float InactiveRouteFramesThreshold()
    {
        return SettingsManager.Get().miscSettings.inactiveRouteFramesThreshold;
    }

    public Dictionary<TinyPlanetResources.PlanetResourceType, int> GetShipment()
    {
        return _shipment;
    }
}