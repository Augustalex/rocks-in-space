using System;
using System.Collections.Generic;
using System.Linq;
using GameNotifications;
using Interactors;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private static ProgressManager _instance;

    private readonly HashSet<TinyPlanetResources.PlanetResourceType> _gotResources = new();
    private readonly HashSet<BuildingType> _builtBuildings = new();
    private readonly HashSet<PlanetId> _ports = new();

    public enum ColonyProgress
    {
        Zero = 0,
        Started = 1,
        Surviving = 2, // Skipped
        Comfortable = 3,
        Luxurious = 4
    };

    private ColonyProgress _colonyProgress = ColonyProgress.Zero;
    private bool _gotFirstGadgets;

    private const float ShowHintAfterTime = 45;
    
    private float _builtFirstRefineryAt = -1f;
    private bool _hasSentIronHint;
    
    private float _builtFirstFactoryAt = -1f;
    private bool _hasSentFactoryHint;
    
    private float _builtFirstCopperRefineryAt = -1f;
    private bool _hasSentCopperHint;

    public event Action LanderBuilt;
    public event Action<TinyPlanetResources.PlanetResourceType> OnResourceGot;

    public static ProgressManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public void Built(BuildingType buildingType)
    {
        if (buildingType == BuildingType.Refinery && !HasBuilt(BuildingType.Refinery))
        {
            _builtFirstRefineryAt = Time.time;
        }
        
        if (buildingType == BuildingType.CopperRefinery && !HasBuilt(BuildingType.CopperRefinery))
        {
            _builtFirstCopperRefineryAt = Time.time;
        }

        if (buildingType == BuildingType.Factory && !HasBuilt(BuildingType.Factory))
        {
            _builtFirstFactoryAt = Time.time;
        }

        if (buildingType == BuildingType.Lander)
        {
            LanderBuilt?.Invoke();
        }

        _builtBuildings.Add(buildingType);
    }

    public void BuiltPort(PlanetId planetId)
    {
        _ports.Add(planetId);
    }

    public void DestroyedPort(PlanetId planetId)
    {
        _ports.Remove(planetId);
    }

    public bool CanTrade()
    {
        return _ports.Count >= 2;
    }

    public bool HasBuilt(BuildingType buildingType)
    {
        return _builtBuildings.Contains(buildingType);
    }

    public int TotalColonistsCount()
    {
        return PlanetsRegistry.Get().All().Sum(planet => planet.GetResources().GetInhabitants());
    }

    public void UpdateProgress()
    {
        ManageIronHint();
        ManageCopperRefineryHint();
        // ManageFactoryHint(); TODO: Re-evaluate if this is really useful. Seems enough with copper and iron hints.

        var happyColonists = 0;
        var overjoyedColonists = 0;

        foreach (var tinyPlanet in PlanetsRegistry.Get().All())
        {
            var colonistsMonitor = tinyPlanet.GetColonistMonitor();
            var status = colonistsMonitor.GetPlanetStatus();
            var colonists = tinyPlanet.GetResources().GetInhabitants();

            if (status == PlanetColonistMonitor.PlanetStatus.Happy)
            {
                happyColonists += colonists;
            }
            else if (status == PlanetColonistMonitor.PlanetStatus.Overjoyed)
            {
                overjoyedColonists += colonists;
            }
        }

        if (_colonyProgress < ColonyProgress.Started && HasBuilt(BuildingType.Port))
        {
            _colonyProgress = ColonyProgress.Started;
        }

        if (_colonyProgress < ColonyProgress.Comfortable &&
            happyColonists >= 2000)
        {
            _colonyProgress = ColonyProgress.Comfortable;
        }

        if (_colonyProgress < ColonyProgress.Comfortable &&
            overjoyedColonists >= 10000)
        {
            _colonyProgress = ColonyProgress.Luxurious;
        }
    }

    private void ManageIronHint()
    {
        if (_hasSentIronHint) return;

        var alreadyGotIron = _gotResources.Contains(TinyPlanetResources.PlanetResourceType.IronOre);
        if (alreadyGotIron) return;

        var hasBuiltRefinery = _builtFirstRefineryAt >= 0;
        if (!hasBuiltRefinery) return;

        var duration = Time.time - _builtFirstRefineryAt;
        var timeToShowHint = duration > ShowHintAfterTime;
        if (!timeToShowHint) return;

        _hasSentIronHint = true;

        var ironPlatesText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.IronPlates);
        var ironOreText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.IronOre);
        Notifications.Get().Send(new TextNotification
        {
            Message =
                $"One of the materials needed to make {ironPlatesText} is {ironOreText}, on some asteroids it is more abundant than on others."
        });
    }

    private void ManageFactoryHint()
    {
        if (_hasSentFactoryHint) return;

        var hasAlreadyGotGadgets = _gotResources.Contains(TinyPlanetResources.PlanetResourceType.CopperOre);
        if (hasAlreadyGotGadgets) return;

        var hasBuiltFactory = _builtFirstFactoryAt >= 0;
        if (!hasBuiltFactory) return;

        var duration = Time.time - _builtFirstFactoryAt;
        var timeToShowHint = duration > ShowHintAfterTime;
        if (!timeToShowHint) return;

        _hasSentFactoryHint = true;

        var gadgetsText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets);
        var copperText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.CopperOre);
        Notifications.Get().Send(new TextNotification
        {
            Message =
                $"Factories need {copperText} to produce {gadgetsText}, on some asteroids it is more abundant than on others."
        });
    }

    private void ManageCopperRefineryHint()
    {
        if (_hasSentCopperHint) return;
        
        var hasAlreadyGotCopperPlates = _gotResources.Contains(TinyPlanetResources.PlanetResourceType.CopperPlates);
        if (hasAlreadyGotCopperPlates) return;
        
        var hasBuiltRefinery = _builtFirstCopperRefineryAt >= 0;
        if (!hasBuiltRefinery) return;
        
        var duration = Time.time - _builtFirstCopperRefineryAt;
        var timeToShowHint = duration > ShowHintAfterTime;
        if (!timeToShowHint) return;
        
        _hasSentCopperHint = true;
        
        var copperPlates =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.CopperPlates);
        var copperText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.CopperOre);
        Notifications.Get().Send(new TextNotification
        {
            Message =
                $"One of the materials needed to make {copperPlates} is {copperText}, on some asteroids it is more abundant than on others."
        });
    }

    public bool LanderUnlocked()
    {
        return HasBuilt(BuildingType.Port);
    }

    public bool RefineryUnlocked()
    {
        return HasBuilt(BuildingType.Lander);
    }

    public bool FactoryUnlocked()
    {
        return HasBuilt(BuildingType.Lander);
    }

    public bool ColonyBasicsProductionUnlocked()
    {
        return _gotFirstGadgets;
    }

    public bool HousingUnlocked()
    {
        return HasBuilt(BuildingType.SolarPanels) && HasBuilt(BuildingType.ProteinFabricator);
    }

    public bool IceProductionUnlocked()
    {
        return Comfortable();
    }

    public bool LuxuryProductionUnlocked()
    {
        return IceProductionUnlocked() && HasBuilt(BuildingType.Purifier);
    }

    public bool EndGameReached()
    {
        return Luxurious();
    }

    public bool Comfortable()
    {
        return _colonyProgress >= ColonyProgress.Comfortable;
    }

    public bool Luxurious()
    {
        return _colonyProgress >= ColonyProgress.Luxurious;
    }

    public void GotFirstGadgets()
    {
        _gotFirstGadgets = true; // Replace with RegisterGotResource
    }

    public void RegisterGotResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        _gotResources.Add(resourceType);
        OnResourceGot?.Invoke(resourceType);
    }

    public bool GotResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        return _gotResources.Contains(resourceType);
    }
}