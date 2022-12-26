using UnityEngine;

public class GameUIController : MonoBehaviour
{
    private CurrentPlanetController _currentPlanetController;

    private DisplayController _displayController;

    void Start()
    {
        _currentPlanetController = CurrentPlanetController.Get();

        _currentPlanetController.CurrentPlanetChanged += ChangeCurrentPlanet;

        _displayController = DisplayController.Get();

        _displayController.ModeChange += mode =>
        {
            if (mode == DisplayController.InputMode.Cinematic)
            {
            }
            else
            {
            }
        };
    }

    private void ChangeCurrentPlanet(PlanetChangedInfo planetChangedInfo)
    {
    }
}
