namespace GameNotifications
{
    public enum NotificationTypes
    {
        Informative,
        Alerting,
        Positive,
        Negative,
        Silent,
    }

    public abstract class Notification
    {
        protected NotificationStatus Status = NotificationStatus.Open;

        protected enum NotificationStatus
        {
            Open,
            Accepted,
            Rejected
        }

        public string Message;
        public NotificationTypes NotificationType = NotificationTypes.Informative;

        public abstract void Accept();

        public abstract void Reject();

        public virtual float Timeout()
        {
            return -1f;
        }
        
        public virtual string Subtitle()
        {
            return "Click to confirm";
        }

        public bool Closed()
        {
            return Status != NotificationStatus.Open;
        }
    }
}