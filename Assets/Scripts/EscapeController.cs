using System;
using Interactors;
using UnityEngine;

public class EscapeController : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (TradeMenu.Get().Visible()) TradeMenu.Get().Dismiss();
            else if (ErrorDisplay.Get().IsVisible() || PlanetPopup.Get().IsVisible())
                PopupManager.Get().CancelAllPopups();
            else if (BuildInteractorIcon.Get().IsBuildMenuOpen())
            {
                BuildInteractorIcon.Get().CloseBuildMenu();
            }
            else if (CameraController.Get().IsZoomedOut())
            {
                CameraController.Get().ZoomIn();
            }
            else if (CurrentPlanetController.Get().IsShipSelected())
            {
                CameraController.Get().ToggleZoomMode();
            }
            else
            {
                InteractorController.Get().SetInteractorByName(SelectInteractor.SelectInteractorName);
            }
        }
    }
}