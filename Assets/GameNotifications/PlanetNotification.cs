namespace GameNotifications
{
    public class PlanetNotification : Notification
    {
        public TinyPlanet Location;
        
        public override string Subtitle()
        {
            return "Click to go there";
        }
        
        public override void Accept()
        {
            CurrentPlanetController.Get().ChangePlanet(Location);
            CameraController.Get().FocusOnPlanet(Location);

            Status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            Status = NotificationStatus.Rejected;
        }
    }
}