using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    private readonly List<Route> _routes = new();
    private static RouteManager _instance;

    private static int _order = 1;

    public static RouteManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        _order = 1;
    }

    void Update()
    {
        foreach (var route in _routes)
        {
            route.Run(Time.deltaTime);
        }
    }

    public void AddRoute(TinyPlanet start, TinyPlanet end)
    {
        var newRoute = new Route(start.PlanetId, end.PlanetId, _order++);
        _routes.Add(newRoute);

        newRoute.Removed += DoRemove;

        void DoRemove()
        {
            if (_routes.Contains(newRoute)) _routes.Remove(newRoute);
            newRoute.Removed -= DoRemove;
        }
    }

    public void RemoveRoute(TinyPlanet start, TinyPlanet end)
    {
        if (!RouteExists(start, end)) return;

        var existingRoute = GetRoute(start, end);
        existingRoute.Remove();
    }

    public bool RouteExists(TinyPlanet start, TinyPlanet end)
    {
        return GetPlanetRoutes(start).Any(r => r.FromTo(start, end));
    }

    public Route GetRoute(TinyPlanet start, TinyPlanet end)
    {
        return GetPlanetRoutes(start).First(r => r.FromTo(start, end));
    }

    public void SetTrade(TinyPlanet start, TinyPlanet end, Dictionary<TinyPlanetResources.PlanetResourceType, int> shipment)
    {
        var route = _routes.Find(route => route.Is(start, end));
        route?.SetTrade(shipment);
    }

    public IEnumerable<Route> GetPlanetRoutes(TinyPlanet planet)
    {
        return _routes.Where(r => r.StartsFrom(planet));
    }

    public Tuple<Route, Route> GetRoutesBetween(TinyPlanet start, TinyPlanet end)
    {
        Route inbound = null;
        Route outbound = null;
        foreach (var route in _routes)
        {
            if (route.FromTo(start, end))
            {
                outbound = route;
            }
            else if (route.FromTo(end, start))
            {
                inbound = route;
            }
        }

        return new Tuple<Route, Route>(inbound, outbound);
    }
}