using System;
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
        var interactorActionCategory =
            InteractorMenuModality.GetCategoryFromInteractorType(InteractorController.Get().CurrentModule()
                .GetInteractorType());

        if (CameraController.Get().IsZoomedOut())
        {
            _buildMenuState = BuildMenuState.ForceClosed;
        }
        else if (CurrentPlanetController.Get().IsShipSelected())
        {
            _buildMenuState = BuildMenuState.ForceClosed;
        }
        else if (interactorActionCategory == InteractorCategory.Build)
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
        bottomBar.HideBuildMenu();
    }

    public void OpenBuildMenu()
    {
        bottomBar.ShowBuildMenu();
    }
}