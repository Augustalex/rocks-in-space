using UnityEngine;

public class TinyPlanetRockType : MonoBehaviour
{
    private TinyPlanet.RockType _rockType;

    public bool IsIce()
    {
        return _rockType == TinyPlanet.RockType.Ice;
    }

    public void Set(TinyPlanet.RockType newRockType)
    {
        _rockType = newRockType;
    }

    public TinyPlanet.RockType Get()
    {
        return _rockType;
    }

    public bool IsCopper()
    {
        return _rockType == TinyPlanet.RockType.Orange;
    }
}