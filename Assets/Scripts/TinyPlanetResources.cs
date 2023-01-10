using System;
using System.Collections;
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
    }

    public enum ResourceTrend
    {
        doubleDown,
        down,
        neutral,
        up,
        doubleUp,
    }

    public static string ResourceName(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Ore: return "Ore";
            case PlanetResourceType.Metals: return "Metals";
            case PlanetResourceType.Gadgets: return "Gadgets";
            case PlanetResourceType.Energy: return "Power";
            case PlanetResourceType.Food: return "Food";
            case PlanetResourceType.Inhabitants: return "Colonists";
            case PlanetResourceType.Housing: return "Housing";
            case PlanetResourceType.Cash: return "credits";
        }

        return "Unknown resource";
    }

    public struct ResourcesData
    {
        public float Ore;
        public float Energy;
        public float Food;
        public int Inhabitants;
        public float Metals;
        public float Gadgets;
    }

    private const int InhabitantsPerResidency = 100;

    private int _residencies = 0;
    private int _occupiedResidencies = 0;
    private int _inhabitants = 0;

    private readonly ResourceTracker _powerTracker = new();
    private readonly ResourceTracker _foodTracker = ResourceTracker.Signed();

    private readonly ResourceTracker _oreTracker = ResourceTracker.Signed();
    private readonly ResourceTracker _metalsTracker = ResourceTracker.Signed();
    private readonly ResourceTracker _gadgetsTracker = ResourceTracker.Signed();

    private int _powerPlants;
    private int _farms;
    private bool _hasHadDeaths;

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
            _powerTracker.ProgressHistory();
            _foodTracker.ProgressHistory();
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
        }
    }

    public ResourceTrend GetTrend(PlanetResourceType resourceType)
    {
        if (resourceType == PlanetResourceType.Housing) return ResourceTrend.neutral;
        if (resourceType == PlanetResourceType.Energy) return ResourceTrend.neutral;

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
            Energy = GetEnergy(),
            Food = GetFood(),
            Inhabitants = GetInhabitants()
        };
    }

    public void RegisterFarm()
    {
        _farms += 1;
    }

    public void DeregisterFarm()
    {
        _farms -= 0;
    }

    public void RegisterPowerPlant()
    {
        _powerPlants += 1;
    }

    public void DeregisterPowerPlant()
    {
        _powerPlants -= 1;
    }

    public bool HasFarm()
    {
        return _farms > 0;
    }

    public bool HasPowerPlant()
    {
        return _powerPlants > 0;
    }
}