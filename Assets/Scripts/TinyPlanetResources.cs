using UnityEngine;

public class TinyPlanetResources : MonoBehaviour
{
    public enum PlanetResourceType
    {
        Ore,
        Metals,
        Gadgets,
        Energy,
        Food,
        Inhabitants
    }

    public static string ResourceName(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Ore: return "Ore";
            case PlanetResourceType.Metals: return "Metals";
            case PlanetResourceType.Gadgets: return "Gadgets";
            case PlanetResourceType.Energy: return "Energy";
            case PlanetResourceType.Food: return "Food";
            case PlanetResourceType.Inhabitants: return "Inhabitants";
        }

        return "Unknown resource";
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

    public void KillResidencyInhabitants()
    {
        _inhabitants -= InhabitantsPerResidency;
        _occupiedResidencies -= 1;
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
}