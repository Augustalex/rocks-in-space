using System;
using System.Collections;
using System.Collections.Generic;
using Interactors;
using UnityEngine;
using Object = UnityEngine.Object;

public class CurrentPlanetController : MonoBehaviour
{
    private TinyPlanet _currentPlanet;
    private static CurrentPlanetController _instance;
    private SpacersWorkRepository _spacersWorkRepository;

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
            previousPlanet =  previousPlanet,
            newPlanet =  tinyPlanet
        });
    }

    public TinyPlanet CurrentPlanet()
    {
        return _currentPlanet;
    }
}

public struct PlanetChangedInfo
{
    public TinyPlanet previousPlanet;
    public TinyPlanet newPlanet;
}