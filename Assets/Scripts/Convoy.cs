using UnityEngine;

public class Convoy
{
    public int Colonists = 0;
    public int CashReward = 0;
    public float TravelTime = 60 * 3;
    public PlanetId PlanetId;
    private float _started;

    public void Start(float currentTime)
    {
        _started = currentTime;
    }

    public float TimeLeft(float currentTime)
    {
        var duration = currentTime - _started;
        
        return Mathf.Max(0f, TravelTime - duration);
    }
}