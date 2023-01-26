using System;
using UnityEngine;

[Serializable]
public class PlanetId
{
    private static int _nextPlanetId = 1;
    private readonly int _id;

    public PlanetId()
    {
        _id = _nextPlanetId++;
    }

    public bool Is(PlanetId id)
    {
        return id._id == _id;
    }

    public override string ToString()
    {
        return _id.ToString();
    }
}

public class TinyPlanetId : MonoBehaviour
{
    public PlanetId planetId = new();
}