﻿using System;
using Interactors;
using UnityEngine;

public class BuildInteractorIcon : MonoBehaviour
{
    private IconToggle _toggle;

    public GameObject buildMenu;
    public BottomBarController bottomBar;

    private enum BuildMenuState
    {
        Active, // The active interactor is Building related
        Inactive, // No active interactor is related to something you "Build"
        ForceClosed, // Closed because of something else having modality (like the map being open for example)
    }

    private BuildMenuState _buildMenuState = BuildMenuState.Inactive;
    private static BuildInteractorIcon _instance;

    public static BuildInteractorIcon Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;

        _toggle = GetComponent<IconToggle>();
        _toggle.OnToggle += OnToggle;

        foreach (var buildingCard in buildMenu.GetComponentsInChildren<BuildingCard>())
        {
            buildingCard.Clicked += BuildingSelected;
        }
    }

    void Start()
    {
        CameraController.Get().OnToggleZoom += (_) => UpdateStates();
        InteractorController.Get().InteractorSelected += InteractorSelected;
        CurrentPlanetController.Get().CurrentPlanetChanged += (_) => UpdateStates();
        CurrentPlanetController.Get().ShipSelected += (_) => UpdateStates();
        DisplayController.Get().ModeChange += (_) => UpdateStates();
        BottomBarController.Get().OnStateChange += (newState) =>
        {
            if (newState == BottomBarController.BottomBarMenuState.ShipInventory) UpdateStates();
        };

        UpdateStates();
    }

    private void OnToggle()
    {
        if (bottomBar.BuildMenuVisible()) CloseBuildMenu();
        else OpenBuildMenu();

        UpdateStates();
    }

    private void InteractorSelected(InteractorModule interactor)
    {
        UpdateStates();
    }

    private void UpdateStates()
    {
        var isBuildCategory = CurrentInteractorIsBuilding();

        if (CameraController.Get().IsZoomedOut())
        {
            _buildMenuState = BuildMenuState.ForceClosed;
        }
        else if (CurrentPlanetController.Get().IsShipSelected())
        {
            _buildMenuState = BuildMenuState.ForceClosed;
        }
        else if (isBuildCategory)
        {
            _buildMenuState = BuildMenuState.Active;
        }
        else if (bottomBar.BuildMenuVisible())
        {
            _buildMenuState = BuildMenuState.Active;
        }
        else
        {
            _buildMenuState = BuildMenuState.Inactive;
        }

        switch (_buildMenuState)
        {
            case BuildMenuState.Active:
                _toggle.gameObject.SetActive(true);
                _toggle.SetOn();
                break;
            case BuildMenuState.Inactive:
                _toggle.gameObject.SetActive(true);
                _toggle.SetOff();
                break;
            case BuildMenuState.ForceClosed:
                CloseBuildMenu();
                _toggle.SetOff();
                _toggle.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool CurrentInteractorIsBuilding()
    {
        var interactorController = InteractorController.Get();
        if (interactorController.CurrentInteractorIsGeneralBuilding())
        {
            return true;
        }
        else
        {
            var currentInteractor = interactorController.CurrentModule();
            var interactorType = currentInteractor
                .GetInteractorType();
            var interactorActionCategory =
                InteractorMenuModality.GetCategoryFromInteractorType(interactorType);
            return interactorActionCategory == InteractorCategory.Build;
        }
    }

    private void BuildingSelected()
    {
        CloseBuildMenu();
        UpdateStates();
    }

    public bool IsBuildMenuOpen()
    {
        return bottomBar.BuildMenuVisible();
    }

    public void CloseBuildMenu()
    {
        bottomBar.HideMenus();
    }

    public void OpenBuildMenu()
    {
        bottomBar.ShowBuildMenu();
    }
}