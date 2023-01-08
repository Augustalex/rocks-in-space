using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class PowerPlantController : MonoBehaviour
{
    private AttachedToPlanet _planetAttachment;

    void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
        _planetAttachment.AttachedTo += OnResourceEffectAttached;
        _planetAttachment.TransferredFromTo += (from, to) =>
        {
            OnResourceEffectDetached(from);
            OnResourceEffectAttached(to);
        };
        _planetAttachment.DetachedFrom += OnResourceEffectDetached;
    }

    private void OnResourceEffectDetached(TinyPlanetResources resources)
    {
        resources.DeregisterPowerPlant();
    }

    private void OnResourceEffectAttached(TinyPlanetResources resources)
    {
        resources.RegisterPowerPlant();
    }
}