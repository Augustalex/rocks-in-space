using System;
using System.Linq;
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

        Setup();

        select.onClick.AddListener(SelfClicked);

        _progressLock = GetComponentInChildren<ProgressLock>();
        _progressLock.CheckLock(buildingType);
    }

    private void SelfClicked()
    {
        var interactorController = InteractorController.Get();
        if (InteractorController.GeneralBuildings().Contains(buildingType))
        {
            interactorController.SetInteractorByBuildingType(buildingType);
        }
        else
        {
            interactorController
                .SetInteractorByInteractorType(InteractorController.FromBuildingType(buildingType));
        }

        Clicked?.Invoke();
    }

    private void Setup()
    {
        var interactorModule = InteractorController.Get().GetGenericInteractorByBuildingType(buildingType);
        Header(interactorModule);
        Cost(interactorModule);
        Upkeep(interactorModule.template);
        Description(interactorModule);
    }

    private void Header(InteractorModule interactor)
    {
        header.text = interactor.GetInteractorName();
    }

    private void Cost(InteractorModule interactor)
    {
        var interactorCosts = interactor.costs;

        var text = "";
        if (interactorCosts.cash > 0)
        {
            text += $"{interactorCosts.cash}<sprite name=\"coin\"> ";
        }

        if (interactorCosts.ore > 0)
        {
            text += $"{interactorCosts.ore}<sprite name=\"ore\"> ";
        }

        if (interactorCosts.ironPlates > 0)
        {
            text += $"{interactorCosts.ironPlates}<sprite name=\"metals\"> ";
        }

        if (interactorCosts.gadgets > 0)
        {
            text += $"{interactorCosts.gadgets}<sprite name=\"gadgets\"> ";
        }

        costs.text = text;
    }

    private void Upkeep(GameObject template)
    {
        var text = "";
        var effect = template.GetComponent<ResourceEffect>();
        if (effect)
        {
            if (effect.energy < 0)
            {
                text += $"{effect.energy}<sprite name=\"power\">  ";
            }

            if (effect.workersNeeded > 0)
            {
                text += $"{effect.workersNeeded} workers  ";
            }
        }

        var runningCosts = template.GetComponent<RunningResourceEffect>();
        if (runningCosts)
        {
            if (runningCosts.cashPerMinute < 0)
            {
                text += $"{runningCosts.cashPerMinute}<sprite name=\"coin\">/min ";
            }
        }

        if (text == "")
        {
            upkeep.gameObject.SetActive(false);
        }
        else
        {
            upkeep.gameObject.SetActive(true);
            upkeep.text = $"{text}";
        }
    }

    private void Description(InteractorModule interactorModule)
    {
        var buildingDescription = interactorModule.template.GetComponent<BuildingDescription>();
        if (!buildingDescription)
            throw new Exception(
                $"Interactor \"{interactorModule.GetInteractorName()}\" is missing a BuildingDescription component.");
        description.text = buildingDescription.GetDescription();
    }
}