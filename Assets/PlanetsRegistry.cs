using System.Collections.Generic;
using UnityEngine;

public class PlanetsRegistry : MonoBehaviour
{
    private readonly Dictionary<PlanetId, TinyPlanet> _planetRegistry = new();
    private readonly List<TinyPlanet> _allPlanets = new();
    private readonly List<PortController> _ports = new();
    private static PlanetsRegistry _instance;

    public static PlanetsRegistry Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    public TinyPlanet FindPlanetById(PlanetId convoyPlanetId)
    {
        return !_planetRegistry.ContainsKey(convoyPlanetId) ? null : _planetRegistry[convoyPlanetId];
    }

    public void Add(PlanetId planetId, TinyPlanet tinyPlanet)
    {
        _planetRegistry.Add(planetId, tinyPlanet);
        _allPlanets.Add(tinyPlanet);
    }

    public void Add(PortController port)
    {
        _ports.Add(port);
    }

    public void Remove(PortController port)
    {
        _ports.Remove(port);
    }

    public int CurrentPortCount()
    {
        return _ports.Count;
    }

    public IEnumerable<TinyPlanet> All()
    {
        return _planetRegistry.Values;
    }

    public TinyPlanet RandomPlanet()
    {
        return _allPlanets[Random.Range(0, _allPlanets.Count)];
    }
}