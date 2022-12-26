using System;
using UnityEngine;

public class CurrentPlanetController : MonoBehaviour
{
    private TinyPlanet _currentPlanet;
    private static CurrentPlanetController _instance;

    public event Action<PlanetChangedInfo> CurrentPlanetChanged;

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
        CurrentPlanetChanged?.Invoke(new PlanetChangedInfo
        {
            PreviousPlanet =  previousPlanet,
            NewPlanet =  tinyPlanet
        });
    }

    public TinyPlanet CurrentPlanet()
    {
        return _currentPlanet;
    }
}

public struct PlanetChangedInfo
{
    public TinyPlanet PreviousPlanet;
    public TinyPlanet NewPlanet;
}