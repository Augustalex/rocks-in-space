using System;
using UnityEngine;

public class CurrentPlanetController : MonoBehaviour
{
    private TinyPlanet _currentPlanet;
    private static CurrentPlanetController _instance;
    private ColonyShip _currentShip;

    public event Action<PlanetChangedInfo> CurrentPlanetChanged;
    public event Action<ColonyShip> ShipSelected;

    public static CurrentPlanetController Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public void ChangePlanet(TinyPlanet tinyPlanet)
    {
        var previousPlanet = _currentPlanet;

        _currentPlanet = tinyPlanet;
        _currentShip = null;

        CurrentPlanetChanged?.Invoke(new PlanetChangedInfo
        {
            PreviousPlanet = previousPlanet,
            NewPlanet = tinyPlanet
        });
    }

    public TinyPlanet CurrentPlanet()
    {
        return _currentPlanet;
    }

    public void FocusOnShip(ColonyShip colonyShip)
    {
        _currentPlanet = null;
        _currentShip = colonyShip;
        
        ShipSelected?.Invoke(colonyShip);
    }

    public ColonyShip CurrentShip()
    {
        return _currentShip;
    }
}

public struct PlanetChangedInfo
{
    public TinyPlanet PreviousPlanet;
    public TinyPlanet NewPlanet;
}