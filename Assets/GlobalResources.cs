using UnityEngine;

public class GlobalResources : MonoBehaviour
{
    private static GlobalResources _instance;

    private double _cash = 10000;

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
}