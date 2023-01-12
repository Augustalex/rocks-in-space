namespace GameNotifications
{
    public class MapNotification : Notification
    {
        public override void Accept()
        {
            CameraController.Get().ZoomOut();

            _status = NotificationStatus.Accepted;
        }

        public override void Reject()
        {
            _status = NotificationStatus.Rejected;
        }
    }
}