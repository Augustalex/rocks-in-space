using UnityEngine;

[RequireComponent(typeof(TinyPlanet))]
public class PlanetColonistMonitor : MonoBehaviour
{
    private TinyPlanet _planet;

    public enum PlanetStatus
    {
        Uninhabited,
        MovingOut, // Death
        Neutral, // No income
        Happy, // Income
        Overjoyed // Double income
    }

    public enum ColonistStatus
    {
        MovingOut, // Death
        Neutral, // No income
        Happy, // Income
        Overjoyed // Double income
    }

    private PlanetStatus _planetStatus = PlanetStatus.Uninhabited;

    private int _power;
    private int _food;
    private int _refreshments;

    private float _frameHouseIncome;
    private float _buffer;
    private float _bufferFrameTime;
    private float _estimatedHouseIncome;

    void Start()
    {
        _planet = GetComponent<TinyPlanet>();
    }

    void Update()
    {
        var resources = _planet.GetResources();
        if (resources.GetInhabitants() == 0)
        {
            SetStatus(PlanetStatus.Uninhabited);
        }
        else
        {
            var hasEnergy = _power >= 0;
            var hasFood = _food >= 0;
            var hasRefreshments = _refreshments >= 0;
            if (hasEnergy && hasFood && hasRefreshments)
            {
                SetStatus(PlanetStatus.Overjoyed);
            }
            else if (hasEnergy && hasFood)
            {
                SetStatus(PlanetStatus.Happy);
            }
            else if (hasEnergy || hasFood)
            {
                SetStatus(PlanetStatus.Neutral);
            }
            else
            {
                SetStatus(PlanetStatus.MovingOut);
            }
        }


        _buffer += _frameHouseIncome;
        _bufferFrameTime += Time.deltaTime;

        if (_bufferFrameTime >= 1f)
        {
            _estimatedHouseIncome = _buffer;
            _buffer = 0f;
            _bufferFrameTime = 0f;
        }

        _frameHouseIncome = 0f;

        _power = 0;
        _food = 0;
        _refreshments = 0;
    }

    public float GetHouseIncomeEstimate()
    {
        return _estimatedHouseIncome;
    }

    private void SetStatus(PlanetStatus status)
    {
        _planetStatus = status;
    }

    public PlanetStatus GetPlanetStatus()
    {
        return _planetStatus;
    }

    public ColonistStatus CalculateStatus(bool hasEnergy, bool hasFood, bool hasRefreshments)
    {
        if (hasEnergy && hasFood && hasRefreshments)
        {
            return ColonistStatus.Overjoyed;
        }

        if (hasEnergy && hasFood)
        {
            return ColonistStatus.Happy;
        }

        if (hasEnergy || hasFood)
        {
            return ColonistStatus.Neutral;
        }

        return ColonistStatus.MovingOut;
    }

    public void RegisterFoodSatisfied()
    {
        _food += 1;
    }

    public void RegisterNotEnoughFood()
    {
        _food -= 1;
    }

    public void RegisterPowerSatisfied()
    {
        _power += 1;
    }

    public void RegisterNotEnoughPower()
    {
        _power -= 1;
    }

    public void RegisterRefreshmentsSatisfied()
    {
        _refreshments += 1;
    }

    public void RegisterNotEnoughRefreshments()
    {
        _refreshments -= 1;
    }

    public void RegisterHouseIncome(float cashEffectSecond)
    {
        _frameHouseIncome += cashEffectSecond;
    }
}