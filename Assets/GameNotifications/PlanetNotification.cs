public class PlanetNotification : Notification
{
    public TinyPlanet location;

    public override void Accept()
    {
        CurrentPlanetController.Get().ChangePlanet(location);
    }
}