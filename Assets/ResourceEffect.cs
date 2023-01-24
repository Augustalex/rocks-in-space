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
    private bool _attached;

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
        if (_attached) return;

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

        _attached = true;
    }

    private void DetachFrom(TinyPlanetResources resources)
    {
        if (!_attached) return;

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

        _attached = false;
    }

    public void Pause()
    {
        DetachFrom(_planetAttachment.GetAttachedResources());
    }

    public bool Paused()
    {
        return !_attached;
    }

    public void Resume()
    {
        AttachTo(_planetAttachment.GetAttachedResources());
    }
}