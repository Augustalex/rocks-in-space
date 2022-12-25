using System.Collections;
using System.Collections.Generic;
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
    
    private static int _ore = 0;
    private static int _metals = 0;
    private static int _gadgets = 0;
    private static int _energy = 0;
    private static int _inhabitants = 0;
    private static int _food = 0;

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
    
    public void SetEnergy(int newOre)
    {
        _energy = newOre;
    }

    public int GetEnergy()
    {
        return _energy;
    }
    
    public void SetFood(int newOre)
    {
        _food = newOre;
    }

    public int GetFood()
    {
        return _food;
    }
    
    public void SetInhabitants(int newOre)
    {
        _inhabitants = newOre;
    }

    public int GetInhabitants()
    {
        return _inhabitants;
    }
}
