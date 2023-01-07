using System;
using System.Collections.Generic;
using System.Linq;
using Interactors;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private static ProgressManager _instance;

    private readonly HashSet<BuildingType> _builtBuildings = new();

    public static ProgressManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Built(BuildingType buildingType)
    {
        _builtBuildings.Add(buildingType);
    }

    public bool HasBuilt(BuildingType buildingType)
    {
        return _builtBuildings.Contains(buildingType);
    }

    public int TotalColonistsCount()
    {
        return PlanetsRegistry.Get().All().Sum(planet => planet.GetResources().GetInhabitants());
    }
}