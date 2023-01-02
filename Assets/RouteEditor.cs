using System;
using Interactors;
using UnityEngine;

public class RouteEditor : MonoBehaviour
{
    private static RouteEditor _instance;
    private TinyPlanet _start;
    private TinyPlanetResources.PlanetResourceType _resourceType = TinyPlanetResources.PlanetResourceType.Ore;
    private float _started;

    public event Action<TinyPlanet> RouteStarted;
    public event Action RouteCancelled;
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

    private void Update()
    {
        if (Time.time - _started > 10)
        {
            CancelEditing();
        }
    }

    private void CancelEditing()
    {
        Reset();
        RouteCancelled?.Invoke();
    }

    public void SelectRouteStart(TinyPlanet start)
    {
        _started = Time.time;
        _start = start;

        RouteStarted?.Invoke(_start);
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

    public void SelectRouteDestination(TinyPlanet end)
    {
        if (!IsValidDestination(end))
        {
            CancelEditing();
        }
        else
        {
            var routeManager = RouteManager.Get();
            if (!routeManager.RouteExists(_start, end))
            {
                routeManager.AddRoute(_start, end);
            }

            routeManager.SetTrade(_start, end, _resourceType, 25);

            RouteFinished?.Invoke(_start, end);
            Reset();
        }
    }

    private void Reset()
    {
        _start = null;
    }

    public bool IsValidDestination(TinyPlanet tinyPlanet)
    {
        if (!_start) return false;

        return !_start.planetId.Is(tinyPlanet.planetId);
    }

    public bool IsEditing()
    {
        return _start != null;
    }
}