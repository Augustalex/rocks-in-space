using System;
using UnityEngine;

public class AttachedToPlanet : MonoBehaviour
{
    public event Action<TinyPlanetResources> AttachedTo;
    public event Action<TinyPlanetResources> DetachedFrom;
    public event Action<TinyPlanetResources, TinyPlanetResources> TransferredFromTo;

    private TinyPlanetResources _resources;

    public void AttachTo(TinyPlanetResources resources)
    {
        _resources = resources;
        AttachedTo?.Invoke(resources);
    }

    public void DetachFrom(TinyPlanetResources resources)
    {
        if (resources != _resources)
        {
            Debug.LogError("Trying to detach resource effect from planet it is not attached to!");
            return;
        }

        DetachedFrom?.Invoke(resources);
    }

    public void TransferTo(TinyPlanetResources target)
    {
        var original = _resources;
        _resources = target;

        TransferredFromTo?.Invoke(original, target);
    }

    public TinyPlanetResources GetAttachedResources()
    {
        return _resources;
    }
}