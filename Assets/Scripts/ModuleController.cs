using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ResourceEffect))]
public class ModuleController : MonoBehaviour
{
    [FormerlySerializedAs("incomePerSecond")]
    public float incomePerMinute;

    private TinyPlanetResources _planetResources;
    private PowerControlled _powerControlled;

    private bool _occupied;
    private float _life = 100f;
    private ResourceEffect _resourceEffect;
    private const float FoodUsedPerSecond = 2f;
    private const float LifeLossPerSecond = 10f;

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
            var foodNeed = FoodUsedPerSecond * Time.deltaTime;
            var food = _planetResources.GetFood();
            var hasEnoughFood = food >= foodNeed;

            if (hasEnoughFood)
            {
                _planetResources.UseFood(foodNeed);
            }

            var hasEnoughEnergy = _planetResources.GetEnergy() > 0f;
            if (!hasEnoughEnergy)
            {
                _powerControlled.PowerOff();
            }
            else if (!_powerControlled.PowerIsOn())
            {
                _powerControlled.PowerOn();
            }

            if (!hasEnoughFood || !hasEnoughEnergy)
            {
                _life -= LifeLossPerSecond * Time.deltaTime;

                if (_life <= 0f)
                {
                    _planetResources.KillResidencyInhabitants();
                    _occupied = false;
                }
            }

            if (hasEnoughFood && hasEnoughEnergy)
            {
                GlobalResources.Get().AddCash(incomePerMinute);
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