using UnityEngine;

[RequireComponent(typeof(ResourceEffect))]
public class ModuleController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private PowerControlled _powerControlled;

    private bool _occupied;
    private float _life = 100f;
    private ResourceEffect _resourceEffect;

    public const float FoodUsedPerMinute = 120f;
    private const float LifeLossPerSecond = 100f / 60f;

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
            var foodNeed = (FoodUsedPerMinute / 60f) * Time.deltaTime;
            var food = _planetResources.GetFood();
            var hasEnoughFood = food >= foodNeed;

            if (hasEnoughFood)
            {
                _planetResources.UseFood(foodNeed);
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
                    _planetResources.KillResidencyInhabitants();
                    _occupied = false;
                }
            }

            if (hasEnoughFood && hasEnoughEnergy)
            {
                var incomePerMinute =
                    (SettingsManager.Get().balanceSettings.houseIncomePerMinute / 60f) * Time.deltaTime;
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