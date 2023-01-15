namespace GameNotifications
{
    public class ConvoyNotification : Notification
    {
        public ColonyShip colonyShip;

        public override void Accept()
        {
            colonyShip.NavigateToShip();
            Status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            Status = NotificationStatus.Rejected;
        }
    }
}