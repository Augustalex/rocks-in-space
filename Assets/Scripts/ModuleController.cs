using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
[RequireComponent(typeof(ResourceEffect))]
public class ModuleController : MonoBehaviour
{
    private PowerControlled _powerControlled;

    private bool _occupied;
    private float _life = 100f;
    private AttachedToPlanet _planetAttachment;

    private const float LifeLossPerSecond = 100f / 60f;

    public float
        foodPerMinute; // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.

    public float
        cashPerMinute; // Is most likely positive, but kept as a neutral variable to make thinking about balance easier.

    void Start()
    {
        _powerControlled = GetComponentInChildren<PowerControlled>();
        _powerControlled.PowerOff();

        _planetAttachment = GetComponent<AttachedToPlanet>();
        _planetAttachment.TransferredFromTo += OnPlanetTransfer;
    }

    void Update()
    {
        var resources = _planetAttachment.GetAttachedResources();

        if (_occupied)
        {
            var foodEffect = (foodPerMinute / 60f) * Time.deltaTime;
            var food = resources.GetFood();
            var hasEnoughFood = food >= foodEffect;

            if (hasEnoughFood)
            {
                resources
                    .AddFood(foodEffect); // Is most likely negative, but kept as a neutral variable to make thinking about balance easier.
            }

            var hasEnoughEnergy = resources.GetEnergy() > 0f;

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
                        resources.RemoveResidencyInhabitants();
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
            var removed = from.RemoveResidencyInhabitants();
            to.AddColonists(removed);
        }
    }
}