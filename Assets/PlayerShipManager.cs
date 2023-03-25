using Game;
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
            _currentPlanet = _shipMover.CurrentPlanet();
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
        if (_currentPlanet == null) return false;
        return currentPlanet.PlanetId.Is(_currentPlanet);
    }
}