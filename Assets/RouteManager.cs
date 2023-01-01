using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RouteManager : MonoBehaviour
{
    private readonly List<Route> _routes = new();
    private static RouteManager _instance;

    public static RouteManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
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
        Debug.Log("ADD ROUTE! " + start.planetName + " -> " + end.planetName);
        _routes.Add(new Route(start.planetId, end.planetId));
    }

    public void SetTrade(TinyPlanet start, TinyPlanet end, TinyPlanetResources.PlanetResourceType resourceType,
        int amountPerSecond)
    {
        var route = _routes.Find(route => route.Is(start, end));
        route?.SetTrade(resourceType, amountPerSecond);
    }

    public IEnumerable<Route> GetPlanetRoutes(TinyPlanet planet)
    {
        return _routes.Where(r => r.StartsFrom(planet));
    }
}