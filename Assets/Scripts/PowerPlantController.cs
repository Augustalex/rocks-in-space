using UnityEngine;

public class PowerPlantController : MonoBehaviour
{
    private ResourceEffect _resourceEffect;

    void Awake()
    {
        _resourceEffect = GetComponent<ResourceEffect>();
        _resourceEffect.AttachedTo += OnResourceEffectAttached;
        _resourceEffect.DetachedFrom += OnResourceEffectDetached;
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
