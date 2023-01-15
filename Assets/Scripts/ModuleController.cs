using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
[RequireComponent(typeof(ResourceEffect))]
public class ModuleController : MonoBehaviour
{
    private PowerControlled _powerControlled;

    private bool _occupied;
    private const float TotalLife = 100f;
    private float _life = TotalLife;
    private AttachedToPlanet _planetAttachment;

    private const float LifeLossPerSecond = 100f / 60f;

    public float
        foodPerMinute; // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.

    public float
        cashPerMinute; // Is most likely positive, but kept as a neutral variable to make thinking about balance easier.

    public string debugStatus = "";

    void Start()
    {
        _powerControlled = GetComponentInChildren<PowerControlled>();
        _powerControlled.PowerOff();

        _planetAttachment = GetComponent<AttachedToPlanet>();
        _planetAttachment.TransferredFromTo += OnPlanetTransfer;
        _planetAttachment.DetachedFrom += OnDetachedToDeath;
    }

    void Update()
    {
        debugStatus = "";

        var cashEffect = (cashPerMinute / 60f) * Time.deltaTime;

        var resources = _planetAttachment.GetAttachedResources();
        var monitor = _planetAttachment.GetAttachedColonistsMonitor();

        if (_occupied)
        {
            var foodEffect = (foodPerMinute / 60f) * Time.deltaTime;
            debugStatus += "foodEffect: " + foodEffect + ", ";
            var food = resources.GetFood();
            debugStatus += "food: " + food + ", ";
            var hasEnoughFood =
                food > .5f; // Food levels can not be below 0, so checking for 0 never happens (since a farm might always be adding just a little bit).
            debugStatus += "hasEnoughFood: " + (hasEnoughFood ? "true" : "false");
            if (hasEnoughFood)
            {
                resources
                    .AddFood(foodEffect); // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.
                monitor.RegisterFoodSatisfied();
            }
            else
            {
                resources
                    .AddFood(foodEffect); // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.
                monitor.RegisterNotEnoughFood();
            }

            var hasEnoughEnergy = resources.GetEnergy() >= 0f;
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

            var status = monitor.CalculateStatus(hasEnoughEnergy, hasEnoughFood);
            if (status == PlanetColonistMonitor.ColonistStatus.MovingOut)
            {
                Debug.Log("MOVINg OUT: " + _life);
                _life -= LifeLossPerSecond * Time.deltaTime;

                if (_life <= 0f)
                {
                    Debug.Log("DEATH!");
                    resources.DeregisterOccupiedResident();
                    resources.RegisterDeath();
                    _occupied = false;
                }
                else if (_life <= 10f)
                {
                    var lifeLoss = Random.Range(LifeLossPerSecond * .5f, LifeLossPerSecond) * Time.deltaTime;
                    Debug.Log("ALMOST DEAD... " + lifeLoss);
                    _life += lifeLoss;
                }
                else
                {
                    var lifeLossPerSecond = LifeLossPerSecond * Time.deltaTime;
                    Debug.Log("Dying: " + lifeLossPerSecond);
                    _life -= lifeLossPerSecond;
                }
            }
            else if (status == PlanetColonistMonitor.ColonistStatus.Neutral)
            {
                GlobalResources.Get().AddCash(cashEffect);
            }
            else if (status == PlanetColonistMonitor.ColonistStatus.Happy)
            {
                _life = Mathf.Min(TotalLife, _life + LifeLossPerSecond * Time.deltaTime);
                GlobalResources.Get().AddCash(cashEffect * 2f);
            }
            else if (status == PlanetColonistMonitor.ColonistStatus.Overjoyed)
            {
                _life = Mathf.Min(TotalLife, _life + LifeLossPerSecond * 2f * Time.deltaTime);
                GlobalResources.Get().AddCash(cashEffect * 2f);
            }
        }
        else if (resources.HasVacancy())
        {
            _life = 100f;
            _powerControlled.PowerOn();
            resources.OccupyResidency();
            _occupied = true;
        }
        else if (_powerControlled.PowerIsOn())
        {
            _powerControlled.PowerOff();
        }
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