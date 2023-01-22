using UnityEngine;

[RequireComponent(typeof(TinyPlanet))]
public class PlanetColonistMonitor : MonoBehaviour
{
    private TinyPlanet _planet;

    public string debug_text = "";

    public enum PlanetStatus
    {
        Uninhabited,
        MovingOut, // Death
        Surviving,
        Neutral,
        Happy,
        Overjoyed
    }

    public enum ColonistStatus
    {
        MovingOut, // Death
        Neutral, // No income
        Surviving,
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
    private int _contentHouse;
    private int _houseAverageContent;

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
            var overallContent = _houseAverageContent >= 0;

            var hasEnergy = _power > 0;
            var hasFood = _food > 0;
            var hasRefreshments = _refreshments > 0;
            if (hasEnergy && hasFood && hasRefreshments)
            {
                SetStatus(PlanetStatus.Overjoyed);
            }
            else if (hasEnergy && (hasFood || hasRefreshments))
            {
                SetStatus(PlanetStatus.Happy);
            }
            else if (hasEnergy)
            {
                SetStatus(PlanetStatus.Neutral);
            }
            else if (overallContent)
            {
                SetStatus(PlanetStatus.Surviving);
            }
            else
            {
                SetStatus(PlanetStatus.MovingOut);
            }
        }

        debug_text = "status: " + _planetStatus + ", content: " + _houseAverageContent;

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

        _houseAverageContent = 0;
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

    public ColonistStatus CalculateStatus(bool isLander = false, bool hasEnergy = false, bool hasFood = false,
        bool hasRefreshments = false)
    {
        if (isLander) return ColonistStatus.Surviving;
        if (hasEnergy && hasFood && hasRefreshments)
        {
            return ColonistStatus.Overjoyed;
        }

        if (hasEnergy && (hasFood || hasRefreshments))
        {
            return ColonistStatus.Happy;
        }

        if (hasEnergy)
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

    public void RegisterContentHouse()
    {
        _houseAverageContent += 1;
    }

    public void RegisterDiscontentHouse()
    {
        _houseAverageContent -= 1;
    }

    public void RegisterHouseIncome(float cashEffectSecond)
    {
        _frameHouseIncome += cashEffectSecond;
    }
}