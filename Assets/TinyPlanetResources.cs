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

    public static int GetGlobalInhabitants()
    {
        return _inhabitants;
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
    
    private static int _ore = 0;
    private static int _metals = 0;
    private static int _gadgets = 0;
    private static float _energy = 0;
    private static int _inhabitants = 0;
    private static int _residencies = 0;
    private static int _occupiedResidencies = 0;
    private static float _food = 0;

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
    
    public void SetEnergy(float newOre)
    {
        _energy = newOre;
    }

    public float GetEnergy()
    {
        return _energy;
    }
    
    public void SetFood(float newOre)
    {
        _food = newOre;
    }

    public float GetFood()
    {
        return _food;
    }
    
    public static void AddColonists(int colonistCount)
    {
        _inhabitants += colonistCount;
    }
    
    public void SetInhabitants(int newOre)
    {
        _inhabitants = newOre;
    }

    public int GetInhabitants()
    {
        return _inhabitants;
    }

    public void AddResidency()
    {
        _residencies += 1;
    }

    public void DestroyVacantResidency()
    {
        _residencies -= 1;
    }
    
    public bool HasVacancy()
    {
        var occupiedInhabitants = _occupiedResidencies * InhabitantsPerResidency;
        return occupiedInhabitants < _inhabitants;
    }

    public static bool HasSpaceForInhabitants(int additionalInhabitants)
    {
        var vacancies = _residencies - _occupiedResidencies;
        var additionalCapacity = vacancies * InhabitantsPerResidency;
        return additionalCapacity >= additionalInhabitants;
    }

    public void OccupyResidency()
    {
        _occupiedResidencies += 1;
    }

    public void VacantResidency()
    {
        _occupiedResidencies -= 1;
    }

    public void KillResidencyInhabitants()
    {
        _inhabitants -= InhabitantsPerResidency;
        _occupiedResidencies -= 1;
    }

    public void DestroyOccupiedResidency()
    {
        KillResidencyInhabitants();
        DestroyVacantResidency();
    }
}
