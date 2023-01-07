namespace GameNotifications
{
    public class ConvoyNotification : Notification
    {
        public ColonyShip colonyShip;

        public override void Accept()
        {
            CurrentPlanetController.Get().FocusOnShip(colonyShip);
            CameraController.Get().FocusOnShip(colonyShip);

            _status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            _status = NotificationStatus.Rejected;
        }
    }
}