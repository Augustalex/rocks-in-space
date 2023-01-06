using UnityEngine;

public class FarmController : MonoBehaviour
{
    private ResourceEffect _resourceEffect;
    private TinyPlanetResources _resources;

    public float foodPerMinute = 10f;

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
            var foodEffect = foodPerMinute / 60f;
            _resources.AddFood(foodEffect * Time.deltaTime);
        }
    }
}