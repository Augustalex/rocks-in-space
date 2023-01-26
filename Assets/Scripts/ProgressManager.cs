using System.Collections.Generic;
using System.Linq;
using GameNotifications;
using Interactors;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    private static ProgressManager _instance;

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

    private const float ShowCopperHintAfterTime = 45;
    private float _builtFirstFactoryAt;
    private bool _hasSentCopperHint;

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
        if (buildingType == BuildingType.Factory && !HasBuilt(BuildingType.Factory))
        {
            _builtFirstFactoryAt = Time.time;
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
        ManageCopperHint();

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

    private void ManageCopperHint()
    {
        if (_hasSentCopperHint || !(_builtFirstFactoryAt > 0) || _gotFirstGadgets) return;

        var duration = Time.time - _builtFirstFactoryAt;
        if (!(duration > ShowCopperHintAfterTime)) return;

        var gadgetsText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets);
        var copperText =
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Copper);
        Notifications.Get().Send(new TextNotification
        {
            Message =
                $"Factories need {copperText} to produce {gadgetsText}. There is a specific kind of asteroid where {copperText} is abundant."
        });
        _hasSentCopperHint = true;
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
        _gotFirstGadgets = true;
    }
}