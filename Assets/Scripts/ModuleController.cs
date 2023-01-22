using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AttachedToPlanet))]
[RequireComponent(typeof(ResourceEffect))]
public class ModuleController : MonoBehaviour
{
    public bool isLander;

    private PowerControlled _powerControlled;

    private bool _occupied;
    private const float TotalLife = 100f;
    private float _life = TotalLife;
    private AttachedToPlanet _planetAttachment;

    private const float LifeLossPerSecond = 100f / 60f;

    public float
        foodPerMinute; // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.

    public float
        refreshmentsPerMinute; // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.

    public float
        cashPerMinute; // Is most likely positive, but kept as a neutral variable to make thinking about balance easier.

    private void Awake()
    {
        _powerControlled = GetComponentInChildren<PowerControlled>();
    }

    void Start()
    {
        if (_powerControlled) _powerControlled.PowerOff();

        _planetAttachment = GetComponent<AttachedToPlanet>();
        _planetAttachment.TransferredFromTo += OnPlanetTransfer;
        _planetAttachment.DetachedFrom += OnDetachedToDeath;
    }

    void Update()
    {
        var resources = _planetAttachment.GetAttachedResources();

        if (_occupied)
        {
            ConsumeResources();
            UpdateMonitor();
            GlobalResources.Get().AddCash(CashEffectSecond());
            UpdateLifeStatus();
        }
        else if (resources.HasUnallocatedInhabitants())
        {
            _life = 100f;
            resources.OccupyResidency();
            _occupied = true;

            if (_powerControlled) _powerControlled.PowerOn();
        }
        else if (_powerControlled)
        {
            if (_powerControlled.PowerIsOn()) _powerControlled.PowerOff();
        }
    }

    private float CashEffectSecond()
    {
        var baseCashEffect = (cashPerMinute / 60f) * Time.deltaTime;
        return GetStatus() switch
        {
            PlanetColonistMonitor.ColonistStatus.Surviving => baseCashEffect,
            PlanetColonistMonitor.ColonistStatus.Neutral => baseCashEffect * 2f,
            PlanetColonistMonitor.ColonistStatus.Happy => baseCashEffect * 3f,
            PlanetColonistMonitor.ColonistStatus.Overjoyed => baseCashEffect * 4f,
            _ => 0f
        };
    }

    private void UpdateLifeStatus()
    {
        var resources = _planetAttachment.GetAttachedResources();

        var status = GetStatus();
        if (status == PlanetColonistMonitor.ColonistStatus.MovingOut)
        {
            _life -= LifeLossPerSecond * Time.deltaTime;

            if (_life <= 0f)
            {
                resources.DeregisterOccupiedResident();
                resources.RegisterDeath();
                _occupied = false;
            }
            else if (_life <= 10f)
            {
                var lifeLoss = Random.Range(LifeLossPerSecond * .5f, LifeLossPerSecond) * Time.deltaTime;
                _life += lifeLoss;
            }
            else
            {
                var lifeLossPerSecond = LifeLossPerSecond * Time.deltaTime;
                _life -= lifeLossPerSecond;
            }
        }
        else if (status == PlanetColonistMonitor.ColonistStatus.Surviving)
        {
            // Do nothing
        }
        else if (status == PlanetColonistMonitor.ColonistStatus.Neutral)
        {
            _life = Mathf.Min(TotalLife, _life + LifeLossPerSecond * Time.deltaTime); // Heal
        }
        else if (status == PlanetColonistMonitor.ColonistStatus.Happy)
        {
            _life = Mathf.Min(TotalLife, _life + LifeLossPerSecond * Time.deltaTime); // Heal
        }
        else if (status == PlanetColonistMonitor.ColonistStatus.Overjoyed)
        {
            _life = Mathf.Min(TotalLife, _life + LifeLossPerSecond * 2f * Time.deltaTime); // Heal
        }
    }

    private PlanetColonistMonitor.ColonistStatus GetStatus()
    {
        if (!_occupied)
        {
            Debug.LogError("Trying to get status of house, even though it is NOT occupied.");
            return PlanetColonistMonitor.ColonistStatus.MovingOut;
        }

        return _planetAttachment.GetAttachedColonistsMonitor()
            .CalculateStatus(isLander, HasEnoughEnergy(), HasEnoughFood(), HasEnoughRefreshments());
    }

    private void ConsumeResources()
    {
        var resources = _planetAttachment.GetAttachedResources();

        if (foodPerMinute != 0)
        {
            var foodEffect = (foodPerMinute / 60f) * Time.deltaTime;
            resources
                .AddFood(foodEffect); // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.
        }

        if (refreshmentsPerMinute != 0)
        {
            var refreshmentsEffect = (refreshmentsPerMinute / 60f) * Time.deltaTime;
            resources
                .AddResource(TinyPlanetResources.PlanetResourceType.Refreshments,
                    refreshmentsEffect); // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.
        }
    }

    private void UpdateMonitor()
    {
        var monitor = _planetAttachment.GetAttachedColonistsMonitor();

        if (foodPerMinute != 0)
        {
            var hasEnoughFood = HasEnoughFood();
            if (hasEnoughFood)
            {
                monitor.RegisterFoodSatisfied();
            }
            else
            {
                monitor.RegisterNotEnoughFood();
            }
        }

        if (refreshmentsPerMinute != 0)
        {
            var hasEnoughRefreshments = HasEnoughRefreshments();
            if (hasEnoughRefreshments)
            {
                monitor.RegisterRefreshmentsSatisfied();
            }
            else
            {
                monitor.RegisterNotEnoughRefreshments();
            }
        }

        if (_powerControlled)
        {
            var hasEnoughEnergy = HasEnoughEnergy();
            if (hasEnoughEnergy)
            {
                if (!_powerControlled.PowerIsOn()) _powerControlled.PowerOn();
                monitor.RegisterPowerSatisfied();
            }
            else
            {
                if (_powerControlled.PowerIsOn()) _powerControlled.PowerOff();
                monitor.RegisterNotEnoughPower();
            }
        }

        var status = monitor.CalculateStatus(isLander, HasEnoughEnergy(), HasEnoughFood(), HasEnoughRefreshments());
        if (status == PlanetColonistMonitor.ColonistStatus.MovingOut) monitor.RegisterDiscontentHouse();
        else monitor.RegisterContentHouse();

        monitor.RegisterHouseIncome(CashEffectSecond());
    }

    private bool HasEnoughFood()
    {
        return
            _planetAttachment.GetAttachedResources().GetFood() >=
            .5f; // Food levels can not be below 0, so checking for 0 never happens (since a farm might always be adding just a little bit).
    }

    private bool HasEnoughRefreshments()
    {
        return _planetAttachment.GetAttachedResources()
                   .GetResource(TinyPlanetResources.PlanetResourceType.Refreshments) >=
               .5f; // Food levels can not be below 0, so checking for 0 never happens (since a farm might always be adding just a little bit).
    }

    private bool HasEnoughEnergy()
    {
        return _planetAttachment.GetAttachedResources().GetEnergy() >= 0f;
    }

    private void OnPlanetTransfer(TinyPlanetResources from, TinyPlanetResources to)
    {
        if (_occupied)
        {
            from.DeregisterOccupiedResident();
            to.RegisterOccupiedResident();
        }
    }

    private void OnDetachedToDeath(TinyPlanetResources current)
    {
        if (_occupied)
        {
            current.DeregisterOccupiedResident();
            current.RegisterDeath();
        }
    }
}