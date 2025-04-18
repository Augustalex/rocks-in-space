using System;
using System.Collections;
using System.Collections.Generic;
using Interactors;
using UnityEngine;

public class TinyPlanetResources : MonoBehaviour
{
    public enum
        PlanetResourceType // Remake to global resource type? Since it includes both planet resource as well as cash (which is global).
    {
        Ore = 0,
        Gadgets = 2,
        Energy = 3,
        Food = 4,
        Inhabitants = 5,
        Housing = 6,
        Cash = 7, // Global, not planet specific.
        Ice = 8,
        Water = 9,
        Refreshments = 10,
        IronOre = 11,
        IronPlates = 1,
        CopperOre = 13,
        CopperPlates = 16,
        Graphite = 12,
        Protein = 14,
        Dangeronium = 15
    }

    public enum ResourceTrend
    {
        DoubleDown,
        Down,
        Neutral,
        Up,
        DoubleUp,
    }

    public static string ResourceName(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Inhabitants: return $"colonists";
            case PlanetResourceType.CopperPlates: return $"copper plates";
            case PlanetResourceType.Dangeronium: return $"Dangeronium";
            default:
                return ResourceNameOnly(resourceType) + ResourceSprite(resourceType);
        }
    }

    public static string ResourceNameOnly(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Ore: return $"ore";
            case PlanetResourceType.Gadgets: return $"gadgets";
            case PlanetResourceType.Energy: return $"power";
            case PlanetResourceType.Food: return $"fresh food";
            case PlanetResourceType.Inhabitants: return $"colonists";
            case PlanetResourceType.Housing: return $"housing";
            case PlanetResourceType.Cash: return $"credits";
            case PlanetResourceType.Ice: return $"ice";
            case PlanetResourceType.Water: return $"water";
            case PlanetResourceType.Refreshments: return $"drinks";
            case PlanetResourceType.IronOre: return $"iron ore";
            case PlanetResourceType.IronPlates: return $"iron plates";
            case PlanetResourceType.CopperOre: return $"copper ore";

            case PlanetResourceType.CopperPlates: return $"copper plates";
            // case PlanetResourceType.CopperPlates: return $"copper plates{ResourceSprite(resourceType)}"; Uncomment when has added copper plates to sprite sheet

            case PlanetResourceType.Graphite: return $"graphite";
            case PlanetResourceType.Protein: return $"protein chunks";
        }

        return "Unknown resource";
    }

    public static string ResourceSprite(PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case PlanetResourceType.Ore: return "<sprite name=\"ore\">";
            case PlanetResourceType.Gadgets: return "<sprite name=\"gadgets\">";
            case PlanetResourceType.Energy: return "<sprite name=\"power\">";
            case PlanetResourceType.Food: return "fresh <sprite name=\"food\">";
            case PlanetResourceType.Inhabitants: return "";
            case PlanetResourceType.Housing: return "<sprite name=\"house\">";
            case PlanetResourceType.Cash: return "<sprite name=\"coin\">";
            case PlanetResourceType.Ice: return "<sprite name=\"ice\">";
            case PlanetResourceType.Water: return "<sprite name=\"water\">";
            case PlanetResourceType.Refreshments: return "<sprite name=\"refreshments\">";
            case PlanetResourceType.IronOre: return "<sprite name=\"iron\">";
            case PlanetResourceType.IronPlates: return "<sprite name=\"metals\">";
            case PlanetResourceType.CopperOre: return "<sprite name=\"copper\">";
            case PlanetResourceType.CopperPlates: return "copper plates";
            case PlanetResourceType.Graphite: return "<sprite name=\"graphite\">";
            case PlanetResourceType.Protein: return "<sprite name=\"protein\">";
        }

        return "";
    }

    public struct ResourcesData
    {
        public float IronOre;
        public float Graphite;
        public float CopperOre;
        public float Energy;
        public float Protein;
        public float Food;
        public int Inhabitants;
        public int Landers;
        public float Ice;
        public float IronPlates;
        public float Gadgets;
        public float Refreshments;
    }

    public static readonly int InhabitantsPerResidency = 100;

    private int _residencies = 0;
    private int _occupiedResidencies = 0;
    private int _inhabitants = 0;

    private readonly ResourceTracker _powerTracker = ResourceTracker.Signed();
    private readonly ResourceTracker _foodTracker = new();

    private readonly ResourceTracker _oreTracker = new();
    private readonly ResourceTracker _iceTracker = new();
    private readonly ResourceTracker _waterTracker = new();
    private readonly ResourceTracker _refreshmentsTracker = new();
    private readonly ResourceTracker _ironOreTracker = new();
    private readonly ResourceTracker _ironPlatesTracker = new();
    private readonly ResourceTracker _copperOreTracker = new();
    private readonly ResourceTracker _copperPlatesTracker = new();
    private readonly ResourceTracker _gadgetsTracker = new();
    private readonly ResourceTracker _graphiteTracker = new();
    private readonly ResourceTracker _proteinTracker = new();

    private Dictionary<PlanetResourceType, ResourceTracker> _resourceTrackers;

    private int _powerPlants;
    private int _farms;
    private int _distilleries;
    private int _purifiers;
    private bool _hasHadDeaths;

    private readonly Dictionary<BuildingType, int> _buildings = new()
    {
        { BuildingType.Port, 0 },
        { BuildingType.Lander, 0 },
        { BuildingType.Refinery, 0 },
        { BuildingType.CopperRefinery, 0 },
        { BuildingType.Factory, 0 },
        { BuildingType.Purifier, 0 },
        { BuildingType.Distillery, 0 },
        { BuildingType.PowerPlant, 0 },
        { BuildingType.SolarPanels, 0 },
        { BuildingType.FarmDome, 0 },
        { BuildingType.ProteinFabricator, 0 },
        { BuildingType.ResidentModule, 0 },
    };

    private int _landers;
    private int _hiredWorkers;

    private void Awake()
    {
        _resourceTrackers = new Dictionary<PlanetResourceType, ResourceTracker>()
        {
            { PlanetResourceType.Ore, _oreTracker },
            { PlanetResourceType.IronOre, _ironOreTracker },
            { PlanetResourceType.IronPlates, _ironPlatesTracker },
            { PlanetResourceType.CopperOre, _copperOreTracker },
            { PlanetResourceType.CopperPlates, _copperPlatesTracker },
            { PlanetResourceType.Graphite, _graphiteTracker },
            { PlanetResourceType.Gadgets, _gadgetsTracker },
            { PlanetResourceType.Energy, _powerTracker },
            { PlanetResourceType.Ice, _iceTracker },
            { PlanetResourceType.Water, _waterTracker },
            { PlanetResourceType.Refreshments, _refreshmentsTracker },
            { PlanetResourceType.Protein, _proteinTracker },
            { PlanetResourceType.Food, _foodTracker },
        };
    }

    void Start()
    {
        StartCoroutine(RunTrends());
    }

    IEnumerator RunTrends()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(.5f);
            _oreTracker.ProgressHistory();
            _oreTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Ore);

            _ironOreTracker.ProgressHistory();
            _ironOreTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.IronOre);

            _graphiteTracker.ProgressHistory();
            _graphiteTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Graphite);

            _copperOreTracker.ProgressHistory();
            _copperOreTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.CopperOre);

            _ironPlatesTracker.ProgressHistory();
            _ironPlatesTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.IronPlates);

            _copperPlatesTracker.ProgressHistory();
            _copperPlatesTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.CopperPlates);

            _gadgetsTracker.ProgressHistory();
            _gadgetsTracker.OnFirstPositiveChange += () => ProgressManager.Get().GotFirstGadgets();
            _gadgetsTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Gadgets);

            // _powerTracker.ProgressHistory();
            _proteinTracker.ProgressHistory();
            _proteinTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Protein);

            _foodTracker.ProgressHistory();
            _foodTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Food);

            _iceTracker.ProgressHistory();
            _iceTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Ice);

            _waterTracker.ProgressHistory();
            _waterTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Water);

            _refreshmentsTracker.ProgressHistory();
            _refreshmentsTracker.OnFirstPositiveChange +=
                () => ProgressManager.Get().RegisterGotResource(PlanetResourceType.Refreshments);
        }
    }

    public float GetResource(PlanetResourceType resourceType)
    {
        if (resourceType == PlanetResourceType.Inhabitants) return GetInhabitants();
        if (resourceType == PlanetResourceType.Housing) return GetVacantHousing();
        return GetTracker(resourceType).Get();
    }

    public void RemoveResource(PlanetResourceType resourceType, float amount)
    {
        GetTracker(resourceType).Remove(amount);
    }

    public void AddResource(PlanetResourceType resourceType, float amount)
    {
        GetTracker(resourceType).Add(amount);
    }

    public ResourceTrend GetTrend(PlanetResourceType resourceType)
    {
        if (resourceType == PlanetResourceType.Housing) return ResourceTrend.Neutral;
        if (resourceType == PlanetResourceType.Energy) return ResourceTrend.Neutral;

        return GetTracker(resourceType).GetTrend();
    }

    private ResourceTracker GetTracker(PlanetResourceType resourceType)
    {
        if (!_resourceTrackers.ContainsKey(resourceType))
        {
            throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType,
                "Trying to get tracker for resources that doesnt have any");
        }

        return _resourceTrackers[resourceType];
    }

    public float GetOre()
    {
        return _oreTracker.Get();
    }

    public void RemoveOre(float toTake)
    {
        _oreTracker.Remove(toTake);
    }

    public void AddOre(float toAdd)
    {
        _oreTracker.Add(toAdd);
    }

    public float GetIronPlates()
    {
        return _ironPlatesTracker.Get();
    }

    public void RemoveIronPlates(float toTake)
    {
        _ironPlatesTracker.Remove(toTake);
    }

    public void AddIronPlates(float toAdd)
    {
        _ironPlatesTracker.Add(toAdd);
    }

    public float GetGadgets()
    {
        return _gadgetsTracker.Get();
    }

    public void RemoveGadgets(float toTake)
    {
        _gadgetsTracker.Remove(toTake);
    }

    public void AddGadgets(float toAdd)
    {
        _gadgetsTracker.Add(toAdd);
    }

    public void RemoveEnergy(float energyToRemove)
    {
        _powerTracker.Remove(energyToRemove);
    }

    public void AddEnergy(float energyToAdd)
    {
        _powerTracker.Add(energyToAdd);
    }

    public float GetEnergy()
    {
        return _powerTracker.Get();
    }

    public void AddFood(float food)
    {
        _foodTracker.Add(food);
    }

    public void UseFood(float food)
    {
        _foodTracker.Remove(food);
    }

    public float GetFood()
    {
        return _foodTracker.Get();
    }

    public int GetInhabitants()
    {
        return _inhabitants;
    }

    public bool HasHadDeaths()
    {
        return _hasHadDeaths;
    }

    public void AddColonists(int colonistCount)
    {
        _inhabitants += colonistCount;
    }

    public int DeregisterOccupiedResident()
    {
        var toRemove = InhabitantsPerResidency;
        _inhabitants -= toRemove;

        _occupiedResidencies -= 1;
        _residencies -= 1;

        return toRemove;
    }

    public void RegisterDeath()
    {
        _hasHadDeaths = true;
    }

    public int RegisterOccupiedResident()
    {
        var toAdd = InhabitantsPerResidency;
        _inhabitants += toAdd;
        _occupiedResidencies += 1;
        _residencies += 1;

        return toAdd;
    }

    public void RegisterLander()
    {
        RegisterOccupiedResident();
        RegisterOccupiedResident();
        RegisterOccupiedResident();
        RegisterOccupiedResident();
        RegisterOccupiedResident();

        _landers += InhabitantsPerResidency * 5;
    }

    public void DeregisterLander()
    {
        DeregisterOccupiedResident();
        DeregisterOccupiedResident();
        DeregisterOccupiedResident();
        DeregisterOccupiedResident();
        DeregisterOccupiedResident();

        _landers -= InhabitantsPerResidency * 5;
    }

    public void AddResidency()
    {
        _residencies += 1;
    }

    public void RemoveResidency()
    {
        _residencies -= 1;
    }

    public bool HasUnallocatedInhabitants()
    {
        var occupiedInhabitants = _occupiedResidencies * InhabitantsPerResidency;
        return occupiedInhabitants < _inhabitants;
    }

    public bool HasSpaceForInhabitants(int additionalInhabitants)
    {
        return GetVacantHousing() >= additionalInhabitants;
    }

    public void OccupyResidency()
    {
        _occupiedResidencies += 1;
    }

    public bool HasVacantHousing()
    {
        return _residencies > 0;
    }

    public int GetVacantHousing()
    {
        return GetVacantHousesCount() * InhabitantsPerResidency;
    }

    public int GetTotalHousesCount()
    {
        return _residencies;
    }

    public int GetVacantHousesCount()
    {
        return _residencies - _occupiedResidencies;
    }

    public int GetOccupantHousesCount()
    {
        return _occupiedResidencies;
    }

    public ResourcesData CopyData()
    {
        return new ResourcesData
        {
            IronOre = GetResource(PlanetResourceType.IronOre),
            Graphite = GetResource(PlanetResourceType.Graphite),
            CopperOre = GetResource(PlanetResourceType.CopperOre),
            IronPlates = GetIronPlates(),
            Gadgets = GetGadgets(),
            Ice = GetResource(PlanetResourceType.Ice),
            Energy = GetEnergy(),
            Protein = GetResource(PlanetResourceType.Protein),
            Food = GetFood(),
            Inhabitants = GetInhabitants(),
            Landers = GetLanders()
        };
    }

    private int GetLanders()
    {
        return _landers;
    }

    public bool HasFarm()
    {
        return _farms > 0;
    }

    public bool HasPowerPlant()
    {
        return _powerPlants > 0;
    }

    public bool HasPurifier()
    {
        return _purifiers > 0;
    }

    public bool HasDistillery()
    {
        return _distilleries > 0;
    }

    public bool HasBuilding(BuildingType buildingType)
    {
        return _buildings[buildingType] > 0;
    }

    public bool HasPowerBuilding()
    {
        return _buildings[BuildingType.SolarPanels] > 0 || _buildings[BuildingType.PowerPlant] > 0;
    }

    public void DeregisterBuilding(BuildingType buildingType)
    {
        _buildings[buildingType] -= 1;
    }

    public void RegisterBuilding(BuildingType buildingType)
    {
        _buildings[buildingType] += 1;
    }

    public int GetBuildingCount(BuildingType buildingType)
    {
        return _buildings[buildingType];
    }

    public int GetWorkers()
    {
        return _inhabitants - _hiredWorkers;
    }

    public void HireWorkers(int workers)
    {
        _hiredWorkers += workers;
    }

    public void FireWorkers(int workers)
    {
        _hiredWorkers -= workers;
    }
}