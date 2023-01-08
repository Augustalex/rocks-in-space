using System;
using UnityEngine;

[Serializable]
public enum ResidencyType
{
    Nothing,
    Module
}

[RequireComponent(typeof(AttachedToPlanet))]
public class ResourceEffect : MonoBehaviour
{
    public float energy;
    public ResidencyType residencyType;
    private TinyPlanetResources _resources;

    private AttachedToPlanet _planetAttachment;

    private void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();

        _planetAttachment.AttachedTo += AttachTo;
        _planetAttachment.TransferredFromTo += (from, to) =>
        {
            DetachFrom(from);
            AttachTo(to);
        };
        _planetAttachment.DetachedFrom += DetachFrom;
    }

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
    }

    public TinyPlanetResources GetAttachedPlanet()
    {
        return _resources;
    }
}