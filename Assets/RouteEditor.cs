using System;
using System.Collections.Generic;
using Interactors;
using UnityEngine;

public class RouteEditor : MonoBehaviour
{
    private static RouteEditor _instance;
    private TinyPlanet _start;
    private float _started;
    private TinyPlanet _end;

    private Dictionary<TinyPlanetResources.PlanetResourceType, int> _shipment = new ();
    
    public event Action<TinyPlanet> RouteStarted;
    public event Action RouteCancelled;
    public event Action RouteDestinationSelected;
    public event Action<TinyPlanet, TinyPlanet> RouteFinished;

    public enum RouteEditorState
    {
        Creating,
        Editing,
        Idle
    };

    private RouteEditorState _state = RouteEditorState.Idle;

    public static RouteEditor Get()
    {
        return _instance;
    }

    public void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        // TODO: Put these listeners in a "RouteEditorController" that can live in root on Canvas. Since this is more UI related.
        CameraController.Get().OnNavigationStarted += Cancel;
        CameraController.Get().OnToggleZoom += (bool zoomedOut) => Cancel();
        InteractorController.Get().UnhandledMouseUp += OnUnhandledMouseUp;
    }

    private void OnUnhandledMouseUp()
    {
        Cancel();
    }

    public void Cancel()
    {
        Reset();
        RouteCancelled?.Invoke();
    }

    public void ConfirmRoute()
    {
        var routeManager = RouteManager.Get();
        if (!routeManager.RouteExists(_start, _end))
        {
            routeManager.AddRoute(_start, _end);
        }

        routeManager.SetTrade(_start, _end, GetCurrentShipment());

        RouteFinished?.Invoke(_start, _end);
        Reset();
    }

    public void StartCreatingRouteFrom(TinyPlanet start)
    {
        if (_state != RouteEditorState.Idle) Cancel();

        _state = RouteEditorState.Creating;
        SelectRouteStart(start);
    }

    public void EditRoute(TinyPlanet start, TinyPlanet end)
    {
        if (_state != RouteEditorState.Idle) Cancel();

        _state = RouteEditorState.Editing;
        SelectRouteStart(start);
        SelectRouteDestination(end);
    }

    public TinyPlanet GetRouteStart()
    {
        return _start;
    }

    public void SetShipment(Dictionary<TinyPlanetResources.PlanetResourceType, int> shipment)
    {
        _shipment = shipment;
    }

    public float EstimatedShipmentTime()
    {
        if (!_start || !_end)
            throw new Exception("Trying to estimate shipping time for a route without either start or end");
        
        var distance = _start.GetDistanceTo(_end);
        var distanceSeconds = distance / 20f;
        return Route.GetShipmentTime(_shipment) + Mathf.FloorToInt(distanceSeconds);
    }

    public Dictionary<TinyPlanetResources.PlanetResourceType, int> GetCurrentShipment()
    {
        return _shipment;
    }

    public void SelectRouteDestination(TinyPlanet end)
    {
        if (!IsValidDestination(end))
        {
            Cancel();
        }
        else
        {
            _end = end;

            if (RouteManager.Get().RouteExists(_start, _end))
            {
                _shipment = RouteManager.Get().GetRoute(_start, _end).GetShipment();
            }
            
            RouteDestinationSelected?.Invoke();
        }
    }

    public bool IsValidDestination(TinyPlanet tinyPlanet)
    {
        if (_start == null) return false;

        return !_start.PlanetId.Is(tinyPlanet.PlanetId) && tinyPlanet.HasPort();
    }

    public bool IsEditing()
    {
        return _state == RouteEditorState.Editing && HasLoadedARoute();
    }

    public bool IsCreating()
    {
        return _state == RouteEditorState.Creating && HasLoadedARoute();
    }

    public bool IsIdle()
    {
        return _state == RouteEditorState.Idle;
    }

    public bool HasLoadedARoute()
    {
        return _start != null || _end != null;
    }

    public TinyPlanet GetRouteDestination()
    {
        return _end;
    }

    private void SelectRouteStart(TinyPlanet start)
    {
        if (!start.HasPort())
        {
            Cancel();
        }
        else
        {
            _started = Time.time;
            _start = start;

            RouteStarted?.Invoke(_start);
        }
    }

    private void Reset()
    {
        _start = null;
        _end = null;
        _state = RouteEditorState.Idle;
        _shipment = new(); // Creating a new instance instead of clearing the existing one since the reference is passed down to the Route object.
    }

    private float TradeAmountForResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        var balanceSettings = SettingsManager.Get().balanceSettings;

        switch (resourceType)
        {
            case TinyPlanetResources.PlanetResourceType.Ore:
                return balanceSettings.oreTradeAmount;
            case TinyPlanetResources.PlanetResourceType.IronOre:
                return balanceSettings.ironOreTradeAmount;
            case TinyPlanetResources.PlanetResourceType.Graphite:
                return balanceSettings.graphiteTradeAmount;
            case TinyPlanetResources.PlanetResourceType.CopperOre:
                return balanceSettings.copperOreTradeAmount;
            case TinyPlanetResources.PlanetResourceType.IronPlates:
                return balanceSettings.ironPlatesTradeAmount;
            case TinyPlanetResources.PlanetResourceType.CopperPlates:
                return balanceSettings.copperPlatesTradeAmount;
            case TinyPlanetResources.PlanetResourceType.Gadgets:
                return balanceSettings.gadgetsTradeAmount;
            case TinyPlanetResources.PlanetResourceType.Water:
                return balanceSettings.waterTradeAmount;
            case TinyPlanetResources.PlanetResourceType.Refreshments:
                return balanceSettings.refreshmentsTradeAmount;
            default:
                Debug.LogError("Trying to get trade amount for a resource that is not tradable: " + resourceType);
                return 0f;
        }
    }
}