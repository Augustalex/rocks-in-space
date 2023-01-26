using GameNotifications;
using UnityEngine;

[RequireComponent(typeof(PlanetProductionMonitor))]
[RequireComponent(typeof(PlanetResourceMonitor))]
[RequireComponent(typeof(PlanetColonistMonitor))]
[RequireComponent(typeof(PlanetCostMonitor))]
[RequireComponent(typeof(TinyPlanetRocks))]
[RequireComponent(typeof(TinyPlanetId))]
public class TinyPlanet : MonoBehaviour
{
    public PlanetId PlanetId => _planetId.planetId;

    [HideInInspector] public string planetName = "Unnamed";

    public enum RockType // Todo move to own file
    {
        Orange,
        Blue,
        Green,
        Snow, // Only used on Ice planets, now set as a planet type
        Ice,
    }

    private Vector3 _lastCenterPosition;
    private GameObject _lastCenter;
    private int _networkCountWhenLastCalculatedCenter;

    private TinyPlanetRocks _planetRocks;
    private TinyPlanetId _planetId;
    private TinyPlanetRockType _rockType;
    private PlanetColonistMonitor _colonistMonitor;
    private PlanetResourceMonitor _resourceMonitor;
    private PlanetProductionMonitor _productionMonitor;
    private TinyPlanetResources _planetResources;
    private PlanetCostMonitor _costMonitor;

    private void Awake()
    {
        _planetId = GetComponent<TinyPlanetId>();
        _planetRocks = GetComponent<TinyPlanetRocks>();
        _rockType = GetComponent<TinyPlanetRockType>();

        _planetResources = GetComponent<TinyPlanetResources>();
        _colonistMonitor = GetComponent<PlanetColonistMonitor>();
        _resourceMonitor = GetComponent<PlanetResourceMonitor>();
        _productionMonitor = GetComponent<PlanetProductionMonitor>();
        _costMonitor = GetComponent<PlanetCostMonitor>();

        PlanetsRegistry.Get()?.Add(PlanetId, this);
    }

    public TinyPlanetRocks Network()
    {
        return _planetRocks;
    }

    public TinyPlanetResources GetResources()
    {
        return _planetResources;
    }

    public PlanetColonistMonitor GetColonistMonitor()
    {
        return _colonistMonitor;
    }

    public PlanetProductionMonitor GetProductionMonitor()
    {
        return _productionMonitor;
    }

    public PlanetCostMonitor GetCostMonitor()
    {
        return _costMonitor;
    }

    public Vector3 GetCenter()
    {
        return Network().GetCenter();
    }

    public void AttachPort(PortController port)
    {
        PlanetsRegistry.Get().Add(port, PlanetId);
    }

    public bool HasPort()
    {
        return PlanetsRegistry.Get().HasPort(PlanetId);
    }

    public bool Anonymous()
    {
        return planetName is "Unknown" or "Unnamed";
    }

    public PortController GetPort()
    {
        return PlanetsRegistry.Get().GetPort(PlanetId);
    }

    public bool IsIcePlanet()
    {
        return _rockType.IsIce();
    }
}