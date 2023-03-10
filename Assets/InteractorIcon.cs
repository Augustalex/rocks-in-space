using System;
using Interactors;
using UnityEngine;

public class InteractorIcon : MonoBehaviour
{
    public InteractorCategory category = InteractorCategory.Dig;
    private IconToggle _toggle;

    private enum InteractorIconState
    {
        Active,
        Inactive,
        ForceInactive,
    }

    private InteractorIconState _interactorIconState = InteractorIconState.Inactive;

    void Awake()
    {
        _toggle = GetComponent<IconToggle>();
        _toggle.OnToggle += OnToggle;
    }

    void Start()
    {
        CameraController.Get().OnToggleZoom += (_) => UpdateStates();
        InteractorController.Get().InteractorSelected += (_) => UpdateStates();
        CurrentPlanetController.Get().CurrentPlanetChanged += (_) => UpdateStates();
        CurrentPlanetController.Get().ShipSelected += (_) => UpdateStates();
        DisplayController.Get().ModeChange += (_) => UpdateStates();

        UpdateStates();
    }

    private void OnToggle()
    {
        BuildInteractorIcon.Get().CloseBuildMenu();

        // TODO: Break out into subclasses?
        switch (category)
        {
            case InteractorCategory.Dig:
                DigToggle();
                break;
            case InteractorCategory.Select:
                SelectToggle();
                break;
            case InteractorCategory.Map:
                MapToggle();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        UpdateStates();
    }

    private void DigToggle()
    {
        var interactorController = InteractorController.Get();
        interactorController.SetInteractorByInteractorType(InteractorType.Dig);
        
        BottomBarController.Get().HideMenus();
    }

    private void MapToggle()
    {
        var cameraController = CameraController.Get();
        cameraController.ToggleZoomMode();
        
        BottomBarController.Get().HideMenus();
    }

    private void SelectToggle()
    {
        var interactorController = InteractorController.Get();
        var cameraController = CameraController.Get();
        if (cameraController.IsZoomedOut())
        {
            cameraController.ZoomIn();
            interactorController.SetInteractorByInteractorType(InteractorType.Dig);
        }
        
        BottomBarController.Get().HideMenus();
    }

    private void UpdateStates()
    {
        // TODO: Break out into subclasses?
        switch (category)
        {
            case InteractorCategory.Dig:
                UpdateDigState();
                break;
            case InteractorCategory.Select:
                UpdateSelectState();
                break;
            case InteractorCategory.Map:
                UpdateMapState();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        switch (_interactorIconState)
        {
            case InteractorIconState.Active:
                _toggle.gameObject.SetActive(true);
                _toggle.SetOn();
                break;
            case InteractorIconState.Inactive:
                _toggle.gameObject.SetActive(true);
                _toggle.SetOff();
                break;
            case InteractorIconState.ForceInactive:
                _toggle.SetOff();
                _toggle.gameObject.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void UpdateDigState()
    {
        var interactor = InteractorController.Get().CurrentModule();
        var zoomedOut = CameraController.Get().IsZoomedOut();
        if (zoomedOut)
        {
            _interactorIconState = InteractorIconState.ForceInactive;
        }
        else if (CurrentPlanetController.Get().IsShipSelected())
        {
            _interactorIconState = InteractorIconState.ForceInactive;
        }
        else if (interactor.GetInteractorType() == InteractorType.Dig)
        {
            _interactorIconState = InteractorIconState.Active;
        }
        else
        {
            _interactorIconState = InteractorIconState.Inactive;
        }
    }

    private void UpdateMapState()
    {
        var zoomedOut = CameraController.Get().IsZoomedOut();
        
        if (DisplayController.Get().inputMode is DisplayController.InputMode.InventoryOnly or DisplayController.InputMode.Cinematic)
        {
            _interactorIconState = InteractorIconState.ForceInactive;
        }
        else if (DisplayController.Get().inputMode == DisplayController.InputMode.MapAndInventoryOnly)
        {
            _interactorIconState = InteractorIconState.Inactive;
        }
        else if (zoomedOut)
        {
            _interactorIconState = InteractorIconState.Active;
        }
        else
        {
            _interactorIconState = InteractorIconState.Inactive;
        }
    }

    private void UpdateSelectState()
    {
        var interactor = InteractorController.Get().CurrentModule();
        var zoomedOut = CameraController.Get().IsZoomedOut();

        if (zoomedOut)
        {
            _interactorIconState = InteractorIconState.ForceInactive;
        }
        else if (CurrentPlanetController.Get().IsShipSelected())
        {
            _interactorIconState = InteractorIconState.ForceInactive;
        }
        else if (interactor.GetInteractorType() == InteractorType.Select)
        {
            _interactorIconState = InteractorIconState.Active;
        }
        else
        {
            _interactorIconState = InteractorIconState.Inactive;
        }
    }
}