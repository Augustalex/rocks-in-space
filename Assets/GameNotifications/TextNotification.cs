namespace GameNotifications
{
    public class TextNotification : Notification
    {
        public float TimeoutOverride = -1f;
        
        public override float Timeout()
        {
            return TimeoutOverride;
        }
        
        public override void Accept()
        {
            // Do nothing
        }

        public override void Reject()
        {
            // Do nothing
        }
    }
}