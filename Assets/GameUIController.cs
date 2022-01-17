using UnityEngine;

public class GameUIController : MonoBehaviour
{
    private CurrentPlanetController _currentPlanetController;

    public SpacerCountDisplay spacerCountDisplay;
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
                spacerCountDisplay.gameObject.SetActive(false);
            }
            else
            {
                spacerCountDisplay.gameObject.SetActive(true);
            }
        };
    }

    private void ChangeCurrentPlanet(PlanetChangedInfo planetChangedInfo)
    {
        if (planetChangedInfo.previousPlanet != null)
        {
            planetChangedInfo.previousPlanet.GetComponent<SpacersWorkRepository>().SpacersChanged -= SpacersChanged;
        }
        
        planetChangedInfo.newPlanet.GetComponent<SpacersWorkRepository>().SpacersChanged += SpacersChanged;
        
        UpdateSpacerCount(planetChangedInfo.newPlanet.GetComponent<SpacersWorkRepository>().GetSpacerCount());
    }

    private void SpacersChanged(SpacersInfo spacerInfo)
    {
        UpdateSpacerCount(spacerInfo.count);
    }

    private void UpdateSpacerCount(int spacerInfoCount)
    {
        spacerCountDisplay.DisplayCount(spacerInfoCount);
    }
}
