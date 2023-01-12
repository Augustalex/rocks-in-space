using System;
using UnityEngine;

public class AttachedToPlanet : MonoBehaviour
{
    public event Action<TinyPlanetResources> AttachedTo;
    public event Action<TinyPlanetResources> DetachedFrom;
    public event Action<TinyPlanetResources, TinyPlanetResources> TransferredFromTo;

    private TinyPlanet _planet;

    public void AttachTo(TinyPlanet planet)
    {
        _planet = planet;
        AttachedTo?.Invoke(planet.GetResources());
    }

    public void DetachFrom(TinyPlanet planet)
    {
        if (planet != _planet)
        {
            Debug.LogError("Trying to detach resource effect from planet it is not attached to!");
            return;
        }

        _planet = null;

        DetachedFrom?.Invoke(planet.GetResources());
    }

    public void TransferTo(TinyPlanet target)
    {
        var original = _planet;
        if (original == target) return;

        _planet = target;

        TransferredFromTo?.Invoke(original.GetResources(), target.GetResources());
    }

    public TinyPlanetResources GetAttachedResources()
    {
        return _planet.GetResources();
    }

    public PlanetColonistMonitor GetAttachedColonistsMonitor()
    {
        return _planet.GetColonistMonitor();
    }
}