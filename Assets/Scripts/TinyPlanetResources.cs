using System;
using System.Collections;
using System.Collections.Generic;
using Interactors;
using UnityEngine;

public class TinyPlanetResources : MonoBehaviour
{
    public enum
        PlanetResourceType // Remake to global resource type? Since it includes both planet resource as well as cash (which is global).
    {
        Ore,
        Metals,
        Gadgets,
        Energy,
        Food,
        Inhabitants,
        Housing,
        Cash, // Global, not planet specific.
        Ice,
        Water,
        Refreshments
    }

    public enum ResourceTrend
    {
        DoubleDown,
        Down,
        Neutral,
        Up,
        DoubleUp,
    }

    public static string ResourceName(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Ore: return "ore<sprite name=\"ore\">";
            case PlanetResourceType.Metals: return "metals<sprite name=\"metals\">";
            case PlanetResourceType.Gadgets: return "gadgets<sprite name=\"gadgets\">";
            case PlanetResourceType.Energy: return "power<sprite name=\"power\">";
            case PlanetResourceType.Food: return "fresh food<sprite name=\"food\">";
            case PlanetResourceType.Inhabitants: return "colonists";
            case PlanetResourceType.Housing: return "housing<sprite name=\"house\">";
            case PlanetResourceType.Cash: return "credits<sprite name=\"coin\">";
            case PlanetResourceType.Ice: return "ice<sprite name=\"ice\">";
            case PlanetResourceType.Water: return "water<sprite name=\"water\">";
            case PlanetResourceType.Refreshments: return "drinks<sprite name=\"refreshments\">";
        }

        return "Unknown resource";
    }

    public struct ResourcesData
    {
        public float Ore;
        public float Energy;
        public float Food;
        public int Inhabitants;
        public float Ice;
        public float Metals;
        public float Gadgets;
        public float Refreshments;
    }

    public static readonly int InhabitantsPerResidency = 100;

    private int _residencies = 0;
    private int _occupiedResidencies = 0;
    private int _inhabitants = 0;

    private readonly ResourceTracker _powerTracker = ResourceTracker.Signed();
    private readonly ResourceTracker _foodTracker = new();

    private readonly ResourceTracker _oreTracker = new();
    private readonly ResourceTracker _metalsTracker = new();
    private readonly ResourceTracker _gadgetsTracker = new();
    private readonly ResourceTracker _iceTracker = new();
    private readonly ResourceTracker _waterTracker = new();
    private readonly ResourceTracker _refreshmentsTracker = new();

    private int _powerPlants;
    private int _farms;
    private int _distilleries;
    private int _purifiers;
    private bool _hasHadDeaths;

    private readonly Dictionary<BuildingType, int> _buildings = new()
    {
        { BuildingType.Refinery, 0 },
        { BuildingType.Factory, 0 },
        { BuildingType.Purifier, 0 },
        { BuildingType.Distillery, 0 },
        { BuildingType.PowerPlant, 0 },
        { BuildingType.FarmDome, 0 },
    };

    void Start()
    {
        StartCoroutine(RunTrends());
    }

    IEnumerator RunTrends()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(1f);
            _oreTracker.ProgressHistory();
            _metalsTracker.ProgressHistory();
            _gadgetsTracker.ProgressHistory();

            // _powerTracker.ProgressHistory();
            _foodTracker.ProgressHistory();

            _iceTracker.ProgressHistory();
            _waterTracker.ProgressHistory();
            _refreshmentsTracker.ProgressHistory();
        }
    }

    public float GetResource(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Energy:
                return GetEnergy();
            case PlanetResourceType.Food:
                return GetFood();
            case PlanetResourceType.Gadgets:
                return GetGadgets();
            case PlanetResourceType.Inhabitants:
                return GetInhabitants();
            case PlanetResourceType.Metals:
                return GetMetals();
            case PlanetResourceType.Ore:
                return GetOre();
            case PlanetResourceType.Housing:
                return GetVacantHousing();
            case PlanetResourceType.Ice:
                return _iceTracker.Get();
            case PlanetResourceType.Water:
                return _waterTracker.Get();
            case PlanetResourceType.Refreshments:
                return _refreshmentsTracker.Get();
        }

        Debug.LogError("Trying to get resource that has not getter: " + resourceType);
        return 0f;
    }

    public void RemoveResource(PlanetResourceType resourceType, float amount)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Energy:
                RemoveEnergy(amount);
                break;
            case PlanetResourceType.Food:
                UseFood(amount);
                break;
            case PlanetResourceType.Gadgets:
                RemoveGadgets(amount);
                break;
            case PlanetResourceType.Metals:
                RemoveMetals(amount);
                break;
            case PlanetResourceType.Ore:
                RemoveOre(amount);
                break;
            case PlanetResourceType.Ice:
                _iceTracker.Remove(amount);
                break;
            case PlanetResourceType.Water:
                _waterTracker.Remove(amount);
                break;
            case PlanetResourceType.Refreshments:
                _refreshmentsTracker.Remove(amount);
                break;
        }
    }

    public void AddResource(PlanetResourceType resourceType, float amount)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Energy:
                AddEnergy(amount);
                break;
            case PlanetResourceType.Food:
                AddFood(amount);
                break;
            case PlanetResourceType.Gadgets:
                AddGadgets(amount);
                break;
            case PlanetResourceType.Metals:
                AddMetals(amount);
                break;
            case PlanetResourceType.Ore:
                AddOre(amount);
                break;
            case PlanetResourceType.Ice:
                _iceTracker.Add(amount);
                break;
            case PlanetResourceType.Water:
                _waterTracker.Add(amount);
                break;
            case PlanetResourceType.Refreshments:
                _refreshmentsTracker.Add(amount);
                break;
        }
    }

    public ResourceTrend GetTrend(PlanetResourceType resourceType)
    {
        if (resourceType == PlanetResourceType.Housing) return ResourceTrend.Neutral;
        if (resourceType == PlanetResourceType.Energy) return ResourceTrend.Neutral;

        return GetTracker(resourceType).GetTrend();
    }

    private ResourceTracker GetTracker(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Energy:
                return _powerTracker;
            case PlanetResourceType.Food:
                return _foodTracker;
            case PlanetResourceType.Gadgets:
                return _gadgetsTracker;
            case PlanetResourceType.Metals:
                return _metalsTracker;
            case PlanetResourceType.Ore:
                return _oreTracker;
            case PlanetResourceType.Ice:
                return _iceTracker;
            case PlanetResourceType.Water:
                return _waterTracker;
            case PlanetResourceType.Refreshments:
                return _refreshmentsTracker;
        }

        throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType,
            "Trying to get tracker for resources that doesnt have any");
    }

    public float GetOre()
    {
        return _oreTracker.Get();
    }

    public void RemoveOre(float toTake)
    {
        _oreTracker.Remove(toTake);
    }

    public void AddOre(float toAdd)
    {
        _oreTracker.Add(toAdd);
    }

    public float GetMetals()
    {
        return _metalsTracker.Get();
    }

    public void RemoveMetals(float toTake)
    {
        _metalsTracker.Remove(toTake);
    }

    public void AddMetals(float toAdd)
    {
        _metalsTracker.Add(toAdd);
    }

    public float GetGadgets()
    {
        return _gadgetsTracker.Get();
    }

    public void RemoveGadgets(float toTake)
    {
        _gadgetsTracker.Remove(toTake);
    }

    public void AddGadgets(float toAdd)
    {
        _gadgetsTracker.Add(toAdd);
    }

    public void RemoveEnergy(float energyToRemove)
    {
        _powerTracker.Remove(energyToRemove);
    }

    public void AddEnergy(float energyToAdd)
    {
        _powerTracker.Add(energyToAdd);
    }

    public float GetEnergy()
    {
        return _powerTracker.Get();
    }

    public void AddFood(float food)
    {
        _foodTracker.Add(food);
    }

    public void UseFood(float food)
    {
        _foodTracker.Remove(food);
    }

    public float GetFood()
    {
        return _foodTracker.Get();
    }

    public int GetInhabitants()
    {
        return _inhabitants;
    }

    public bool HasHadDeaths()
    {
        return _hasHadDeaths;
    }

    public void AddColonists(int colonistCount)
    {
        _inhabitants += colonistCount;
    }

    public int DeregisterOccupiedResident()
    {
        var toRemove = InhabitantsPerResidency;
        _inhabitants -= toRemove;
        _occupiedResidencies -= 1;

        return toRemove;
    }

    public void RegisterDeath()
    {
        _hasHadDeaths = true;
    }

    public int RegisterOccupiedResident()
    {
        var toAdd = InhabitantsPerResidency;
        _inhabitants += toAdd;
        _occupiedResidencies += 1;

        return toAdd;
    }

    public void AddResidency()
    {
        _residencies += 1;
    }

    public void RemoveResidency()
    {
        _residencies -= 1;
    }

    public bool HasVacancy()
    {
        var occupiedInhabitants = _occupiedResidencies * InhabitantsPerResidency;
        return occupiedInhabitants < _inhabitants;
    }

    public bool HasSpaceForInhabitants(int additionalInhabitants)
    {
        return GetVacantHousing() >= additionalInhabitants;
    }

    public void OccupyResidency()
    {
        _occupiedResidencies += 1;
    }

    public int GetVacantHousing()
    {
        var vacancies = _residencies - _occupiedResidencies;
        var additionalCapacity = vacancies * InhabitantsPerResidency;

        return additionalCapacity;
    }

    public ResourcesData CopyData()
    {
        return new ResourcesData
        {
            Ore = GetOre(),
            Metals = GetMetals(),
            Gadgets = GetGadgets(),
            Ice = GetResource(PlanetResourceType.Ice),
            Energy = GetEnergy(),
            Food = GetFood(),
            Inhabitants = GetInhabitants()
        };
    }

    public bool HasFarm()
    {
        return _farms > 0;
    }

    public bool HasPowerPlant()
    {
        return _powerPlants > 0;
    }

    public bool HasPurifier()
    {
        return _purifiers > 0;
    }

    public bool HasDistillery()
    {
        return _distilleries > 0;
    }

    public bool HasBuilding(BuildingType buildingType)
    {
        return _buildings[buildingType] > 0;
    }

    public void DeregisterBuilding(BuildingType buildingType)
    {
        _buildings[buildingType] -= 1;
    }

    public void RegisterBuilding(BuildingType buildingType)
    {
        _buildings[buildingType] += 1;
    }

    public void GetBuildingCount(BuildingType buildingType)
    {
    }
}