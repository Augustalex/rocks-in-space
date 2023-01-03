using UnityEngine;

public class GlobalResources : MonoBehaviour
{
    private static GlobalResources _instance;

    private double _cash = 0f;

    private void Awake()
    {
        _cash = SettingsManager.Get().balanceSettings.startingCredits;
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