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
            if (hasEnergy && hasFood)
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

        _power = 0;
        _food = 0;
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
}