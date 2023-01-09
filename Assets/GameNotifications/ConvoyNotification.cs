namespace GameNotifications
{
    public class ConvoyNotification : Notification
    {
        public ColonyShip colonyShip;

        public override void Accept()
        {
            colonyShip.NavigateToShip();
            _status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            _status = NotificationStatus.Rejected;
        }
    }
}