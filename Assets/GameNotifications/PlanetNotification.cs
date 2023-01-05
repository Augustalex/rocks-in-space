using UnityEngine;

public class PlanetNotification : Notification
{
    public TinyPlanet location;

    public override void Accept()
    {
        CurrentPlanetController.Get().ChangePlanet(location);
        CameraController.Get().FocusOnPlanet(location);

        _status = NotificationStatus.Accepted;
    }

    public override void Reject()
    {
        _status = NotificationStatus.Rejected;
    }
}