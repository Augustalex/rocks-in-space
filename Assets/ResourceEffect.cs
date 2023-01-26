using System;
using UnityEngine;

[Serializable]
public enum ResidencyType
{
    Nothing,
    Module,
    Lander,
}

[RequireComponent(typeof(AttachedToPlanet))]
public class ResourceEffect : MonoBehaviour
{
    public float energy;
    public int workersNeeded;
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
        resources.HireWorkers(workersNeeded);

        switch (residencyType)
        {
            case ResidencyType.Nothing:
                break;
            case ResidencyType.Module:
                resources.AddResidency();
                break;
            case ResidencyType.Lander:
                resources.RegisterLander();
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
        resources.FireWorkers(workersNeeded);

        switch (residencyType)
        {
            case ResidencyType.Nothing:
                break;
            case ResidencyType.Module:
                resources.RemoveResidency();
                break;
            case ResidencyType.Lander:
                resources.DeregisterLander();

                resources.RegisterDeath();
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