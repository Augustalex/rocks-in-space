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
            route.Run();
        }
    }

    public void AddRoute(TinyPlanet start, TinyPlanet end)
    {
        _routes.Add(new Route(start.planetId, end.planetId, _order++));
    }

    public bool RouteExists(TinyPlanet start, TinyPlanet end)
    {
        return GetPlanetRoutes(start).Any(r => r.FromTo(start, end));
    }

    public Route GetRoute(TinyPlanet start, TinyPlanet end)
    {
        return GetPlanetRoutes(start).First(r => r.FromTo(start, end));
    }

    public void SetTrade(TinyPlanet start, TinyPlanet end, TinyPlanetResources.PlanetResourceType resourceType,
        float amountPerSecond)
    {
        var route = _routes.Find(route => route.Is(start, end));
        route?.SetTrade(resourceType, amountPerSecond);
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