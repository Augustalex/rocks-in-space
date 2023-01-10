using System.Collections.Generic;
using UnityEngine;

public class ResourceTracker
{
    private float _amount;
    private readonly Queue<float> _history = new();
    private readonly bool _signed;
    private const int HistorySize = 9;

    public ResourceTracker(bool signed = false)
    {
        _signed = signed;
    }

    public static ResourceTracker Signed()
    {
        return new ResourceTracker(true);
    }

    public void ProgressHistory()
    {
        _history.Enqueue(_amount);
        if (_history.Count > HistorySize) _history.Dequeue();
    }

    public float Get()
    {
        return _amount;
    }

    public void Add(float x)
    {
        if (_signed) Set(_amount + x);
        else Set(Mathf.Max(0f, _amount + x));
    }

    public void Remove(float x)
    {
        if (_signed) Set(_amount - x);
        else Set(Mathf.Max(0f, _amount - x));
    }

    private void Set(float x)
    {
        var originalAmount = _amount;
        _amount = x;
        UpdateHistory(_amount);
    }

    private void UpdateHistory(float diff)
    {
        // _history.Enqueue(diff);
        // if (_history.Count > HistorySize) _history.Dequeue();
    }

    public TinyPlanetResources.ResourceTrend GetTrend()
    {
        if (_history.Count < 3)
        {
            return TinyPlanetResources.ResourceTrend.neutral;
        }

        var history = _history.ToArray();

        // float totalA = 0;
        // for (var i = 0; i < history.Length - 2; i++)
        // {
        //     var current = history[i];
        //     var next = history[i + 1];
        //     totalA += next - current;
        // }

        float totalB = 0;
        for (var i = 0; i < history.Length - 1; i++)
        {
            var current = history[i];
            var next = history[i + 1];
            totalB += next - current;
        }

        // var average = totalB - totalA;
        var average = totalB;
        Debug.Log(average);
        if (average < -10) return TinyPlanetResources.ResourceTrend.doubleDown;
        if (average < 0) return TinyPlanetResources.ResourceTrend.down;
        if (average > 10) return TinyPlanetResources.ResourceTrend.doubleUp;
        if (average > 0) return TinyPlanetResources.ResourceTrend.up;
        return TinyPlanetResources.ResourceTrend.neutral;
    }
}