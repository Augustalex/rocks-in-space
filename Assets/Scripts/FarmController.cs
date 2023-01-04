using UnityEngine;

public class FarmController : MonoBehaviour
{
    private ResourceEffect _resourceEffect;
    private TinyPlanetResources _resources;

    public const float FoodPerSecond = 10f;

    void Start()
    {
        _resourceEffect = GetComponent<ResourceEffect>();
        _resources = _resourceEffect.GetAttachedPlanet();
        _resourceEffect.AttachedTo += OnResourceEffectAttached;
        _resourceEffect.DetachedFrom += OnResourceEffectDetached;
    }

    private void OnResourceEffectDetached(TinyPlanetResources resources)
    {
        _resources = resources;
    }

    private void OnResourceEffectAttached(TinyPlanetResources resources)
    {
        _resources = null;
    }

    void Update()
    {
        if (!_resources)
        {
            Debug.LogError("This farm is not attached to any planet!");
            return;
        }

        var energy = _resources.GetEnergy();
        if (energy >= 0)
        {
            _resources.AddFood(FoodPerSecond * Time.deltaTime);
        }
    }
}