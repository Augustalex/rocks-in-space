using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class ResourceConversionEffect : MonoBehaviour
{
    public event Action OnStarted;
    public event Action OnStopped;
    public event Action OnSlowedDown;
    public event Action OnResumedSpeed;

    public bool copperPlanetBonus = false;

    public TinyPlanetResources.PlanetResourceType from;
    public int fromAmount = 1;
    public TinyPlanetResources.PlanetResourceType fromSecondary;
    public int fromSecondaryAmount = 0;
    public int bufferSize = 1;

    public TinyPlanetResources.PlanetResourceType to;
    public int toAmount = 1;

    public float iterationTime = 1f;

    public static readonly float ResourceTakeTime = .5f;

    public static readonly float
        SlowDownFactor =
            5f; // This means that process times will be doubled, if it is 2. Sync this number with the animation slow down in the ConversionAnimationController.

    private AttachedToPlanet _planetAttachment;
    private ResourceEffect _resourceEffect;
    private bool _started = false;
    private bool _slowedDown = false;

    private RegisterBuildingOnPlanet _buildingRegister; // Might be null

    // Buffer
    private int _bufferedSets = 0;

    private void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
        _resourceEffect = GetComponent<ResourceEffect>();
        _buildingRegister = GetComponent<RegisterBuildingOnPlanet>();

        _planetAttachment.DetachedFrom += (resources) => Detached(resources.GetComponent<TinyPlanet>());
        _planetAttachment.AttachedTo += (resources) => Attached(resources.GetComponent<TinyPlanet>());
        _planetAttachment.TransferredFromTo += TransferredFromTo;
    }

    private void TransferredFromTo(TinyPlanetResources planetDetachedResources,
        TinyPlanetResources planetAttachedResources)
    {
        var planetDetached = planetDetachedResources.GetComponent<TinyPlanet>();
        Detached(planetDetached);

        var planetAttached = planetAttachedResources.GetComponent<TinyPlanet>();
        Attached(planetAttached);
    }

    void Start()
    {
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (gameObject != null)
        {
            var resources = _planetAttachment.GetAttachedResources();

            if (_resourceEffect)
            {
                var requiresPower = _resourceEffect.energy < 0;
                if (requiresPower)
                {
                    while (resources.GetResource(TinyPlanetResources.PlanetResourceType.Energy) < 0)
                    {
                        Stopped();

                        // Wait until there is power, then continue processing.
                        yield return new WaitForSeconds(.25f);
                    }
                }

                var requiresWorkers = _resourceEffect.workersNeeded > 0;
                if (requiresWorkers)
                {
                    if (resources.GetWorkers() < 0)
                    {
                        SlowedDown();
                    }
                    else
                    {
                        ResumeSpeed();
                    }
                }
            }

            while (gameObject != null &&
                   (!HasEnoughOfPrimaryFromResource(resources) || !HasEnoughOfSecondaryFromResource(resources)))
            {
                Stopped();

                // Wait until there is resources to take from. Then restart the iteration timer.
                yield return new WaitForSeconds(.25f);
            }

            while (_bufferedSets < bufferSize &&
                   HasEnoughOfPrimaryFromResource(resources) &&
                   HasEnoughOfSecondaryFromResource(resources))
            {
                resources.RemoveResource(from, fromAmount);
                resources.RemoveResource(fromSecondary, fromSecondaryAmount);
                _bufferedSets += 1;

                yield return new WaitForSeconds(_slowedDown ? ResourceTakeTime * SlowDownFactor : ResourceTakeTime);
            }

            Started();

            while (_bufferedSets > 0)
            {
                _bufferedSets -= 1;
                yield return new WaitForSeconds(_slowedDown ? iterationTime * SlowDownFactor : iterationTime);

                if (copperPlanetBonus && _planetAttachment.GetAttachedPlanetType().IsCopper())
                {
                    resources.AddResource(to, toAmount * 2f);
                }
                else
                {
                    resources.AddResource(to, toAmount);
                }
            }
        }
    }

    private void ResumeSpeed()
    {
        if (!_slowedDown) return;
        _slowedDown = false;

        if (_buildingRegister)
        {
            var buildingType = _buildingRegister.GetBuildingType();
            _planetAttachment.GetAttachedProductionMonitor()
                .RegisterProductionResumeSpeed(buildingType);
        }

        OnResumedSpeed?.Invoke();
    }

    private void SlowedDown()
    {
        if (_slowedDown) return;
        _slowedDown = true;

        if (_buildingRegister)
        {
            var buildingType = _buildingRegister.GetBuildingType();
            _planetAttachment.GetAttachedProductionMonitor()
                .RegisterProductionSlowDown(buildingType);
        }

        OnSlowedDown?.Invoke();
    }

    private void Started()
    {
        if (_started) return;
        _started = true;

        if (_buildingRegister)
        {
            var buildingType = _buildingRegister.GetBuildingType();
            _planetAttachment.GetAttachedProductionMonitor()
                .RegisterProductionStart(buildingType);
        }

        OnStarted?.Invoke();
    }

    private void Stopped()
    {
        if (!_started) return;
        _started = false;

        RegisterStopped();

        OnStopped?.Invoke();
    }

    private void RegisterStopped(bool silently = false)
    {
        if (_buildingRegister)
        {
            var buildingType = _buildingRegister.GetBuildingType();
            _planetAttachment.GetAttachedProductionMonitor()
                .RegisterProductionStop(buildingType, silently);
        }
    }

    private void Detached(TinyPlanet planet)
    {
        if (_buildingRegister)
        {
            var buildingType = _buildingRegister.GetBuildingType();
            planet.GetProductionMonitor()
                .BuildingWasDetached(buildingType, _started);
        }
    }

    private void Attached(TinyPlanet planet)
    {
        if (_buildingRegister)
        {
            var buildingType = _buildingRegister.GetBuildingType();
            planet.GetProductionMonitor()
                .BuildingWasAttached(buildingType, _started);
        }

        OnStopped?.Invoke();
    }

    private bool HasEnoughOfPrimaryFromResource(TinyPlanetResources resources)
    {
        return resources.GetResource(from) >= fromAmount;
    }

    private bool HasEnoughOfSecondaryFromResource(TinyPlanetResources resources)
    {
        if (fromSecondaryAmount == 0) return true;
        return resources.GetResource(fromSecondary) >= fromSecondaryAmount;
    }
}