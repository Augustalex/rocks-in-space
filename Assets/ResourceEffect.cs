using System;
using UnityEngine;

public class ResourceEffect : MonoBehaviour
{
    public event Action<TinyPlanetResources> AttachedTo;
    public event Action<TinyPlanetResources> DetachedFrom;

    [Serializable]
    public enum ResidencyType
    {
        Nothing,
        Module
    }
    
    public float energy;
    public ResidencyType residencyType;
    private TinyPlanetResources _resources;

    public void AttachTo(TinyPlanetResources resources)
    {
        _resources = resources;
        
        resources.AddEnergy(energy);
        
        switch (residencyType)
        {
            case ResidencyType.Nothing:
                break;
            case ResidencyType.Module:
                resources.AddResidency();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        AttachedTo?.Invoke(resources);
    }
    
    public void DetachFrom(TinyPlanetResources resources)
    {
        if (resources != _resources)
        {
            Debug.LogError("Trying to detach resource effect from planet it is not attached to!");
            return;
        }
        
        resources.RemoveEnergy(energy);
        
        switch (residencyType)
        {
            case ResidencyType.Nothing:
                break;
            case ResidencyType.Module:
                resources.RemoveResidency();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        DetachedFrom?.Invoke(resources);
    }

    public TinyPlanetResources GetAttachedPlanet()
    {
        return _resources;
    }
}
