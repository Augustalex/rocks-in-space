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

    private void AttachTo(TinyPlanetResources resources)
    {
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

    private void DetachFrom(TinyPlanetResources resources)
    {
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
}