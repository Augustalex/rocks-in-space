namespace GameNotifications
{
    public class PlanetNotification : Notification
    {
        public TinyPlanet Location;
        public float TimeoutOverride = -1f;

        public override float Timeout()
        {
            return TimeoutOverride;
        }
        
        public override string Subtitle()
        {
            return "Click to go there";
        }
        
        public override void Accept()
        {
            CurrentPlanetController.Get().ChangePlanet(Location);

            Status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            Status = NotificationStatus.Rejected;
        }
    }
}