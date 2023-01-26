using System.Collections.Generic;
using UnityEngine;

public class PlanetsRegistry : MonoBehaviour
{
    private readonly Dictionary<PlanetId, TinyPlanet> _planetRegistry = new();
    private readonly Dictionary<PlanetId, PortController> _planetPortRegistry = new();
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

    public void Add(PortController port, PlanetId planetId)
    {
        _ports.Add(port);
        _planetPortRegistry[planetId] = port;
    }

    public void Remove(PortController port, PlanetId planetId)
    {
        _ports.Remove(port);
        _planetPortRegistry.Remove(planetId);
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

    public void RemovePlanet(TinyPlanet tinyPlanet)
    {
        _planetRegistry.Remove(tinyPlanet.PlanetId);
        _allPlanets.Remove(tinyPlanet);
    }

    public void RemovePlanet(PlanetId planetId)
    {
        _planetRegistry.Remove(planetId);
        _allPlanets.RemoveAll(p => p.PlanetId == planetId);
    }

    public bool HasPort(PlanetId planetId)
    {
        return _planetPortRegistry.ContainsKey(planetId);
    }

    public PortController GetPort(PlanetId planetId)
    {
        return _planetPortRegistry[planetId];
    }

    public TinyPlanet GetPlanet(PlanetId targetPlanetId)
    {
        return _planetRegistry[targetPlanetId];
    }
}