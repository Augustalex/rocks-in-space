using UnityEngine;

public class GlobalResources : MonoBehaviour
{
    private static GlobalResources _instance;
    
    private double _cash = 1000;
    private int _ore = 0;
    private int _metals = 0;
    private int _gadgets = 0;

    private void Awake()
    {
        _instance = this;
    }

    public static GlobalResources Get()
    {
        return _instance;
    }

    public void AddCash(double cash)
    {
        _cash += cash;
    }

    public void UseCash(double cash)
    {
        _cash -= cash;
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
}
