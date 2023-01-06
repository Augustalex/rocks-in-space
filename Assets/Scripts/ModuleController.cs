using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ResourceEffect))]
public class ModuleController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private PowerControlled _powerControlled;

    private bool _occupied;
    private float _life = 100f;
    private ResourceEffect _resourceEffect;

    // public const float FoodUsedPerMinute = 120f;
    private const float LifeLossPerSecond = 100f / 60f;

    public float
        foodPerMinute; // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.

    public float
        cashPerMinute; // Is most likely positive, but kept as a neutral variable to make thinking about balance easier.

    void Start()
    {
        _powerControlled = GetComponentInChildren<PowerControlled>();
        _powerControlled.PowerOff();

        _resourceEffect = GetComponent<ResourceEffect>();
        _planetResources = _resourceEffect.GetAttachedPlanet();
        _resourceEffect.DetachedFrom += OnResourceEffectDetached;
    }

    void Update()
    {
        if (_occupied)
        {
            var foodEffect = (foodPerMinute / 60f) * Time.deltaTime;
            var food = _planetResources.GetFood();
            var hasEnoughFood = food >= foodEffect;

            if (hasEnoughFood)
            {
                _planetResources
                    .AddFood(foodEffect); // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.
            }

            var hasEnoughEnergy = _planetResources.GetEnergy() > 0f;

            if (hasEnoughEnergy)
            {
                if (!_powerControlled.PowerIsOn()) _powerControlled.PowerOn();
            }
            else
            {
                if (_powerControlled.PowerIsOn()) _powerControlled.PowerOff();
            }

            if (!hasEnoughFood || !hasEnoughEnergy)
            {
                _life -= LifeLossPerSecond * Time.deltaTime;

                if (_life <= 0f)
                {
                    var shouldDieNow = Random.value < .2f;
                    if (shouldDieNow)
                    {
                        // This random number will help make sure not all houses die on the same frame.
                        // It gives the player more breathing room, but also a bigger chance to the needs to balance out before to many people die.
                        _planetResources.KillResidencyInhabitants();
                        _occupied = false;
                    }
                }
            }

            if (hasEnoughFood && hasEnoughEnergy)
            {
                // Is most likely positive, but kept as a neutral variable to make thinking about balance easier.
                var cashEffect = (cashPerMinute / 60f) * Time.deltaTime;
                GlobalResources.Get().AddCash(cashEffect);
            }
        }
        else if (_planetResources.HasVacancy())
        {
            _life = 100f;
            _powerControlled.PowerOn();
            _planetResources.OccupyResidency();
            _occupied = true;
        }
        else if (_powerControlled.PowerIsOn())
        {
            _powerControlled.PowerOff();
        }
    }

    private void OnResourceEffectDetached(TinyPlanetResources resources)
    {
        if (_occupied)
        {
            _planetResources.KillResidencyInhabitants();
        }
    }
}