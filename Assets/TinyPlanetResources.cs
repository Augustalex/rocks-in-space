using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyPlanetResources : MonoBehaviour
{
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
