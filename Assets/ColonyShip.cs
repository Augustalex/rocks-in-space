using GameNotifications;
using UnityEngine;

public class ColonyShip : MonoBehaviour
{
    public void MouseDown()
    {
        var cameraController = CameraController.Get();
        cameraController.EnterShip();

        Notifications.Get().Send(new TextNotification
        {
            NotificationType = NotificationTypes.Positive,
            Message =
                "Standard issue ship for new colony managers. Lasers, 3D printers, a map & a trunk, it has it all.",
            TimeoutOverride = 20f,
        });

        // if (CurrentPlanetController.Get().CurrentShip() == this)
        // {
        //     var cameraController = CameraController.Get();
        //     if (cameraController.IsZoomedOut()) cameraController.ToggleZoomMode();
        // }
        // else
        // {
        //     NavigateToShip();
        // }
    }
}