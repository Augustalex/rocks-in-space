using UnityEngine;

public class PlayerShipManager : MonoBehaviour
{
    private static PlayerShipManager _instance;

    private PlayerShipMover _shipMover;

    private PlanetId _currentPlanet;

    public static PlayerShipManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _shipMover = new PlayerShipMover();

        _shipMover.StateChanged += StateChanged;

        _instance = this;
    }

    private void StateChanged(PlayerShipMover.ShipState newState)
    {
        if (newState == PlayerShipMover.ShipState.Arrived)
        {
            var currentPlanet = _shipMover.CurrentPlanet();
            _currentPlanet = currentPlanet;

            var planet = PlanetsRegistry.Get().GetPlanet(currentPlanet);
            if (planet && CameraController.Get().IsZoomedOut())
            {
                CurrentPlanetController.Get().ChangePlanet(planet);
                planet.Discover();
            }
        }
        else
        {
            _currentPlanet = null;
        }

        if (_currentPlanet == null)
        {
            DisplayController.Get().ShipMoving();
        }
        else
        {
            DisplayController.Get().ShipOnPlanet(_currentPlanet);
        }
    }

    public PlayerShipMover ShipMover()
    {
        return _shipMover;
    }

    public PlanetId CurrentPlanet()
    {
        return _currentPlanet;
    }

    public bool ShipOnPlanet(TinyPlanet currentPlanet)
    {
        return _currentPlanet != null && currentPlanet != null && currentPlanet.PlanetId.Is(_currentPlanet);
    }
}