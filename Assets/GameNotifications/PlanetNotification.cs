namespace GameNotifications
{
    public class PlanetNotification : Notification
    {
        public TinyPlanet location;

        public override void Accept()
        {
            CurrentPlanetController.Get().ChangePlanet(location);
            CameraController.Get().FocusOnPlanet(location);

            Status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            Status = NotificationStatus.Rejected;
        }
    }
}