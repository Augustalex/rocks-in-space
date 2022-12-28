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

    // public static int GetGlobalInhabitants()
    // {
    //     return _inhabitants;
    // }

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

    private static double _cash = 1000;
    private static int _ore = 0;
    private static int _metals = 0;
    private static int _gadgets = 0;

    private int _residencies = 0;
    private int _occupiedResidencies = 0;
    private float _energy = 0;
    private float _food = 0;
    private int _inhabitants = 0;

    public void AddCash(double cash)
    {
        _cash += cash;
    }

    public void UseCash(double cash)
    {
        _cash -= cash;
    }

    public static double GetGlobalCash()
    {
        return _cash;
    }

    public double GetCash()
    {
        return _cash;
    }

    public void SetOre(int newOre)
    {
        _ore = newOre;
    }

    public int GetOre()
    {
        return _ore;
    }

    public void SetMetals(int newOre)
    {
        _metals = newOre;
    }

    public int GetMetals()
    {
        return _metals;
    }

    public void SetGadgets(int newOre)
    {
        _gadgets = newOre;
    }

    public int GetGadgets()
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

    public void RemoveFood(float food)
    {
        _food -= food;
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
}