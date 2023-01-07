namespace GameNotifications
{
    public class BuildingNotification : Notification
    {
        public override void Accept()
        {
            BuildInteractorIcon.Get().OpenBuildMenu();
        }

        public override void Reject()
        {
            // Do nothing
        }
    }
}