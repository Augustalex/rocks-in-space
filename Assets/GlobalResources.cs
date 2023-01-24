using System.Collections;
using UnityEngine;

public class GlobalResources : MonoBehaviour
{
    private static GlobalResources _instance;

    private readonly ResourceTracker _cashTracker = ResourceTracker.Signed();

    private void Awake()
    {
        _cashTracker.Add(SettingsManager.Get().balanceSettings.startingCredits);
        _instance = this;
    }

    public static GlobalResources Get()
    {
        return _instance;
    }

    void Start()
    {
        StartCoroutine(RunTrends());
    }

    IEnumerator RunTrends()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(1f);
            _cashTracker.ProgressHistory();
        }
    }

    public void AddCash(double cash)
    {
        _cashTracker.Add((float)cash);
    }

    public void UseCash(double cash)
    {
        _cashTracker.Remove((float)cash);
    }

    public double GetCash()
    {
        return _cashTracker.Get();
    }

    public TinyPlanetResources.ResourceTrend GetTrend()
    {
        return _cashTracker.GetTrend();
    }
}