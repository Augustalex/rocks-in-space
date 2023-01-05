using UnityEngine;

public class FarmController : MonoBehaviour
{
    private ResourceEffect _resourceEffect;
    private TinyPlanetResources _resources;

    public const float FoodPerSecond = 10f;

    void Awake()
    {
        _resourceEffect = GetComponent<ResourceEffect>();
        _resourceEffect.AttachedTo += OnResourceEffectAttached;
        _resourceEffect.DetachedFrom += OnResourceEffectDetached;
    }

    private void OnResourceEffectDetached(TinyPlanetResources resources)
    {
        resources.DeregisterFarm();
        _resources = null;
    }

    private void OnResourceEffectAttached(TinyPlanetResources resources)
    {
        resources.RegisterFarm();
        _resources = resources;
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