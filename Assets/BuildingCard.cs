using System;
using Interactors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingCard : MonoBehaviour
{
    public BuildingType buildingType;

    public TMP_Text header;
    public TMP_Text costs;
    public TMP_Text upkeep;
    public TMP_Text description;

    public Button select;
    private ProgressLock _progressLock;

    public event Action Clicked;

    private void Start()
    {
        GetComponentInChildren<GifDisplay>().frames = GifManager.Get().FramesByBuildingType(buildingType);

        switch (buildingType)
        {
            case BuildingType.Port:
                Port();
                break;
            case BuildingType.Refinery:
                Refinery();
                break;
            case BuildingType.Factory:
                Factory();
                break;
            case BuildingType.PowerPlant:
                PowerPlant();
                break;
            case BuildingType.FarmDome:
                FarmDome();
                break;
            case BuildingType.ResidentModule:
                HousingModule();
                break;
            case BuildingType.Platform:
                Platform();
                break;
            case BuildingType.KorvKiosk:
                KorvKiosk();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        select.onClick.AddListener(SelfClicked);

        _progressLock = GetComponentInChildren<ProgressLock>();
        _progressLock.CheckLock(buildingType);
    }

    private void SelfClicked()
    {
        InteractorController.Get().SetInteractorByInteractorType(FromBuildingType(buildingType));
        Clicked?.Invoke();
    }

    private static InteractorType FromBuildingType(BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Port => InteractorType.Port,
            BuildingType.Refinery => InteractorType.Refinery,
            BuildingType.Factory => InteractorType.Factory,
            BuildingType.PowerPlant => InteractorType.PowerPlant,
            BuildingType.FarmDome => InteractorType.FarmDome,
            BuildingType.ResidentModule => InteractorType.ResidentModule,
            BuildingType.Platform => InteractorType.Platform,
            BuildingType.KorvKiosk => InteractorType.KorvKiosk,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private void Port()
    {
        header.text = "Beacon";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.Port);
        costs.text = $"{interactor.costs.cash}c";
        upkeep.gameObject.SetActive(false);
        description.text =
            "Establishes a settlement on an asteroid when placed. Trade routes can be created between two beacons.";
    }

    private void Refinery()
    {
        header.text = "Refinery";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.Refinery);
        var cost = interactor.costs.cash;
        var runningCosts = interactor.template.GetComponent<RunningResourceEffect>();
        if (runningCosts == null) Debug.LogError("Refinery is missing running costs component.");

        costs.text = $"{cost}c";
        upkeep.text = $"Upkeep: {runningCosts.cashPerMinute}c/min";
        description.text = "Converts ore into metals.";
    }

    private void Factory()
    {
        header.text = "Factory";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.Factory);
        var cost = interactor.costs.cash;
        var runningCosts = interactor.template.GetComponent<RunningResourceEffect>();
        if (runningCosts == null) Debug.LogError("Factory is missing running costs component.");

        costs.text = $"{cost}c";
        upkeep.text = $"Upkeep: {runningCosts.cashPerMinute}c/min";
        description.text = "Converts metals into gadgets.";
    }

    private void PowerPlant()
    {
        header.text = "Power plant";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.PowerPlant);
        var costData = interactor.costs;
        var runningCosts = interactor.template.GetComponent<RunningResourceEffect>();
        if (runningCosts == null) Debug.LogError("Power plant is missing running costs component.");

        var effect = interactor.template.GetComponent<ResourceEffect>();
        if (effect == null) Debug.LogError("Power plant is missing resource effects component.");

        costs.text = $"{costData.gadgets} gadgets";
        upkeep.text = $"Upkeep: {runningCosts.cashPerMinute}c/min";
        description.text = $"Provides {effect.energy} power. Powers farms and residencies.";
    }

    private void FarmDome()
    {
        header.text = "Farms";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.FarmDome);
        var costData = interactor.costs;
        var runningCosts = interactor.template.GetComponent<RunningResourceEffect>();
        if (runningCosts == null) Debug.LogError("Farms is missing running costs component.");

        var effect = interactor.template.GetComponent<ResourceEffect>();
        if (effect == null) Debug.LogError("Farms is missing resource effects component.");

        var controller = interactor.template.GetComponent<FarmController>();
        if (controller == null) Debug.LogError("Farms is missing controller component.");

        costs.text =
            $"{costData.gadgets} gadgets";
        upkeep.text = $"Upkeep: {effect.energy} power {runningCosts.cashPerMinute}c/min";
        description.text =
            $"Produces {controller.foodPerMinute} food/min. Food is consumed by colonists.";
    }

    private void HousingModule()
    {
        header.text = "Housing";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.ResidentModule);
        var costData = interactor.costs;

        var effect = interactor.template.GetComponent<ResourceEffect>();
        if (effect == null) Debug.LogError("Housing module is missing resource effects component.");

        var controller = interactor.template.GetComponent<ModuleController>();
        if (controller == null) Debug.LogError("Housing module is missing controller component.");

        costs.text =
            $"{costData.gadgets} gadgets";
        upkeep.text = $"Upkeep: {effect.energy} power {controller.foodPerMinute} food/min";
        description.text =
            $"Houses 1000 colonists. Generates {controller.cashPerMinute}c/min when colonists have moved in. Only generates income when food and power needs are met.";
    }

    private void Platform()
    {
        header.text = "Scaffolding";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.Platform);
        var cost = interactor.costs.metals;

        costs.text = $"{cost} metals";
        upkeep.gameObject.SetActive(false);
        description.text =
            $"A thin but strong metal frame used for extending building space in a settlement.";
    }

    private void KorvKiosk()
    {
        header.text = "Fluxcapacitor+";
        var interactor = InteractorController.Get().GetInteractor(InteractorType.KorvKiosk);
        var cost = interactor.costs.cash;

        costs.text = $"{cost}c";
        upkeep.gameObject.SetActive(false);
        description.text =
            $"A breakthrough in fluxcapacitation technology. It comes with a free life time subscription of happy memories.";
    }
}