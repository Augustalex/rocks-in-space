using System;
using Interactors;
using UnityEngine;
using UnityEngine.UI;

public class TopMenu : MonoBehaviour
{
    private InteractorController _interactorController;

    public Button dig;
    public Button build;
    public Button select;
    public Button map;

    void Start()
    {
        _interactorController = InteractorController.Get();

        _interactorController.InteractorSelected += InteractorSelected;
        CameraController.Get().OnToggleZoom += ZoomToggled;

        dig.onClick.AddListener(UseDig);
        build.onClick.AddListener(UseBuild);
        select.onClick.AddListener(UseSelect);
        map.onClick.AddListener(UseMap);
    }

    private void ZoomToggled(bool zoomedOut)
    {
        if (zoomedOut) map.Select();
    }
    
    private void InteractorSelected(InteractorModule interactor)
    {
        Debug.Log("SELECTED: " + interactor.GetInteractorName());
        if (interactor.GetInteractorName() == "Dig")
        {
            Debug.Log("SELECT DIG");
            dig.Select();
        }
        else if (interactor.GetInteractorName() == "Select")
        {
            if (!CameraController.Get().IsZoomedOut()) select.Select();
        }
        else
        {
            Debug.Log("SELECT BUILD");
            build.Select();
        }
    }

    public void UseMap()
    {
        CameraController.Get().ToggleZoomMode();
    }

    public void UseSelect()
    {
        _interactorController.SetInteractorByName("Select");
    }

    public void UseDig()
    {
        _interactorController.SetInteractorByName("Dig");
    }

    public void UseBuild()
    {
        _interactorController.SetInteractorByName("Port");
    }
}