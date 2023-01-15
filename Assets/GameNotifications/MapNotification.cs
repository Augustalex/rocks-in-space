namespace GameNotifications
{
    public class MapNotification : Notification
    {
        public override void Accept()
        {
            CameraController.Get().ZoomOut();

            Status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            Status = NotificationStatus.Rejected;
        }
    }
}