using System;
using UnityEngine;

public class AttachedToPlanet : MonoBehaviour
{
    public event Action<TinyPlanetResources> AttachedTo;
    public event Action<TinyPlanetResources> DetachedFrom;
    public event Action<TinyPlanetResources, TinyPlanetResources> TransferredFromTo; // Would be nice if it was planets, and not only the resources here!

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

    public void TransferTo(PlanetId targetPlanetId)
    {
        var planet = PlanetsRegistry.Get().GetPlanet(targetPlanetId);
        if (!planet)
        {
            throw new Exception(
                "Trying to transfer attachment to planet, but cannot find planet in registry. Planet ID: " +
                targetPlanetId);
        }

        TransferTo(planet);
    }

    public void TransferTo(TinyPlanet target)
    {
        var original = _planet;
        if (original == target) return;

        _planet = target;

        TransferredFromTo?.Invoke(original.GetResources(), target.GetResources());
    }

    public TinyPlanetRockType GetAttachedPlanetType()
    {
        return _planet.GetRockType();
    }


    public TinyPlanetResources GetAttachedResources()
    {
        return _planet.GetResources();
    }

    public PlanetColonistMonitor GetAttachedColonistsMonitor()
    {
        return _planet.GetColonistMonitor();
    }

    public PlanetCostMonitor GetAttachedCostMonitor()
    {
        return _planet.GetCostMonitor();
    }

    public PlanetProductionMonitor GetAttachedProductionMonitor()
    {
        return _planet.GetProductionMonitor();
    }
}