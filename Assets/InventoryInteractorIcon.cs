using System;
using Interactors;
using UnityEngine;

public class InventoryInteractorIcon : MonoBehaviour
{
    public BottomBarController bottomBar;
    public GameObject shipInventoryRoot;

    private IconToggle _toggle;

    private enum InventoryMenuState
    {
        Active, // The active interactor is Building related
        Inactive, // No active interactor is related to something you "Build"
        ForceClosed, // Closed because of something else having modality (like the map being open for example)
    }

    private InventoryMenuState _inventoryMenuState = InventoryMenuState.Inactive;
    private static InventoryInteractorIcon _instance;

    public static InventoryInteractorIcon Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;

        _toggle = GetComponent<IconToggle>();
        _toggle.OnToggle += OnToggle;
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
            if (newState == BottomBarController.BottomBarMenuState.BuildMenu) UpdateStates();
        };

        UpdateStates();
    }

    private void OnToggle()
    {
        if (bottomBar.ShipMenuOpen()) CloseMenu();
        else OpenMenu();

        UpdateStates();
    }

    private void InteractorSelected(InteractorModule interactor)
    {
        UpdateStates();
    }

    private void UpdateStates()
    {
        if (CameraController.Get().IsZoomedOut())
        {
            _inventoryMenuState = InventoryMenuState.ForceClosed;
        }
        else if (DisplayController.Get().inputMode == DisplayController.InputMode.Cinematic)
        {
            _inventoryMenuState = InventoryMenuState.ForceClosed;
        }
        else if (DisplayController.Get().inputMode == DisplayController.InputMode.MapAndInventoryOnly)
        {
            _inventoryMenuState = InventoryMenuState.Active;
        }
        else if (!PlayerShipManager.Get().ShipOnPlanet(CurrentPlanetController.Get().CurrentPlanet()))
        {
            _inventoryMenuState = InventoryMenuState.ForceClosed;
        }
        else if (bottomBar.ShipMenuOpen())
        {
            _inventoryMenuState = InventoryMenuState.Active;
        }
        else
        {
            _inventoryMenuState = InventoryMenuState.Inactive;
        }

        switch (_inventoryMenuState)
        {
            case InventoryMenuState.Active:
                _toggle.gameObject.SetActive(true);
                _toggle.SetOn();
                break;
            case InventoryMenuState.Inactive:
                _toggle.gameObject.SetActive(true);
                _toggle.SetOff();
                break;
            case InventoryMenuState.ForceClosed:
                CloseMenu();
                _toggle.SetOff();
                _toggle.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void CloseMenu()
    {
        bottomBar.HideInventoryMenu();
    }

    public void OpenMenu()
    {
        shipInventoryRoot.GetComponentInChildren<ShipInventoryCargoSlots>().UpdateOnShow();

        bottomBar.ShowShipInventory();
    }
}