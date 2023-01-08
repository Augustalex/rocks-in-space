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
    private float _energy = 0;
    private float _food = 0;
    private int _inhabitants = 0;

    private float _ore = 0;
    private float _metals = 0;
    private float _gadgets = 0;

    private int _powerPlants;
    private int _farms;

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

        Debug.LogError("Trying to remove resource that has not remove method: " + resourceType);
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

        Debug.LogError("Trying to remove resource that has not remove method: " + resourceType);
    }

    public float GetOre()
    {
        return _ore;
    }

    public float GetMetals()
    {
        return _metals;
    }

    public float GetGadgets()
    {
        return _gadgets;
    }

    public void RemoveEnergy(float energyToRemove)
    {
        _energy -= energyToRemove;
    }

    public void AddEnergy(float energyToAdd)
    {
        _energy += energyToAdd;
    }

    public float GetEnergy()
    {
        return _energy;
    }

    public void AddFood(float food)
    {
        _food += food;
    }

    public void UseFood(float food)
    {
        _food -= food;
    }

    public float GetFood()
    {
        return _food;
    }

    public int GetInhabitants()
    {
        return _inhabitants;
    }

    public void AddColonists(int colonistCount)
    {
        _inhabitants += colonistCount;
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

    public int RemoveResidencyInhabitants()
    {
        var toRemove = InhabitantsPerResidency;
        _inhabitants -= toRemove;
        _occupiedResidencies -= 1;

        return toRemove;
    }

    public int GetVacantHousing()
    {
        var vacancies = _residencies - _occupiedResidencies;
        var additionalCapacity = vacancies * InhabitantsPerResidency;

        return additionalCapacity;
    }

    public void RemoveOre(float toTake)
    {
        _ore -= toTake;
    }

    public void AddOre(float toAdd)
    {
        _ore += toAdd;
    }

    public void RemoveMetals(float toTake)
    {
        _metals -= toTake;
    }

    public void AddMetals(float toAdd)
    {
        _metals += toAdd;
    }

    public void RemoveGadgets(float toTake)
    {
        _gadgets -= toTake;
    }

    public void AddGadgets(float toAdd)
    {
        _gadgets += toAdd;
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