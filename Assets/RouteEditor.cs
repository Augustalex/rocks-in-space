using System;
using Interactors;
using UnityEngine;

public class RouteEditor : MonoBehaviour
{
    private static RouteEditor _instance;
    private TinyPlanet _start;
    private TinyPlanetResources.PlanetResourceType _resourceType = TinyPlanetResources.PlanetResourceType.Ore;
    private float _started;
    private TinyPlanet _end;

    public event Action<TinyPlanet> RouteStarted;
    public event Action RouteCancelled;
    public event Action RouteDestinationSelected;
    public event Action<TinyPlanet, TinyPlanet> RouteFinished;

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
        CameraController.Get().OnNavigationStarted += CancelEditing;
        CameraController.Get().OnToggleZoom += (bool zoomedOut) => CancelEditing();
        InteractorController.Get().UnhandledMouseUp += OnUnhandledMouseUp;
    }

    private void OnUnhandledMouseUp()
    {
        CancelEditing();
    }

    public void CancelEditing()
    {
        Reset();
        RouteCancelled?.Invoke();
    }

    public void SelectRouteStart(TinyPlanet start)
    {
        if (!start.HasPort())
        {
            CancelEditing();
        }
        else
        {
            _started = Time.time;
            _start = start;

            RouteStarted?.Invoke(_start);
        }
    }

    public TinyPlanet GetRouteStart()
    {
        return _start;
    }

    public void SetResourceType(TinyPlanetResources.PlanetResourceType resourceType)
    {
        _resourceType = resourceType;
    }

    public bool IsRouting()
    {
        return _start != null;
    }

    public void EditRoute(TinyPlanet start, TinyPlanet end)
    {
        SelectRouteStart(start);
        SelectRouteDestination(end);
    }

    public void SelectRouteDestination(TinyPlanet end)
    {
        Debug.Log("SELECT: " + end);
        if (!IsValidDestination(end))
        {
            CancelEditing();
        }
        else
        {
            _end = end;
            RouteDestinationSelected?.Invoke();
        }
    }

    public void ConfirmRoute()
    {
        var routeManager = RouteManager.Get();
        if (!routeManager.RouteExists(_start, _end))
        {
            routeManager.AddRoute(_start, _end);
        }

        routeManager.SetTrade(_start, _end, _resourceType, TradeAmountForResource(_resourceType));

        RouteFinished?.Invoke(_start, _end);
        Reset();
    }

    private float TradeAmountForResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        var balanceSettings = SettingsManager.Get().balanceSettings;

        switch (resourceType)
        {
            case TinyPlanetResources.PlanetResourceType.Ore:
                return balanceSettings.oreTradeAmount;
            case TinyPlanetResources.PlanetResourceType.Metals:
                return balanceSettings.metalsTradeAmount;
            case TinyPlanetResources.PlanetResourceType.Gadgets:
                return balanceSettings.gadgetsTradeAmount;
            default:
                Debug.LogError("Trying to get trade amount for a resource that is not tradable: " + resourceType);
                return 0f;
        }
    }

    private void Reset()
    {
        _start = null;
        _end = null;
    }

    public bool IsValidDestination(TinyPlanet tinyPlanet)
    {
        if (_start == null) return false;

        return !_start.planetId.Is(tinyPlanet.planetId) && tinyPlanet.HasPort();
    }

    public bool IsEditing()
    {
        return _start != null || _end != null;
    }

    public TinyPlanetResources.PlanetResourceType GetSelectedResourceType()
    {
        return _resourceType;
    }

    public TinyPlanet GetRouteDestination()
    {
        return _end;
    }
}