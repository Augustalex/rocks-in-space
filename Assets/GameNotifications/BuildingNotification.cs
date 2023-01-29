namespace GameNotifications
{
    public class BuildingNotification : Notification
    {
        public override float Timeout()
        {
            return 10f;
        }
        
        public override string Subtitle()
        {
            return "Click to show";
        }
        
        public override void Accept()
        {
            var cameraController = CameraController.Get();
            if (cameraController.IsZoomedOut()) cameraController.ZoomIn();
            BuildInteractorIcon.Get().OpenBuildMenu();
        }

        public override void Reject()
        {
            // Do nothing
        }
    }
}