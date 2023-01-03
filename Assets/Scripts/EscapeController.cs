using System;
using Interactors;
using UnityEngine;

public class EscapeController : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ErrorDisplay.Get().IsVisible() || PlanetPopup.Get().IsVisible()) PopupManager.Get().CancelAllPopups();
            else if (!CameraController.Get().IsZoomedOut())
                InteractorController.Get().SetInteractorByName(SelectInteractor.SelectInteractorName);
            else if (CameraController.Get().IsZoomedOut())
            {
                CameraController.Get().ZoomIn();
            }
        }
    }
}