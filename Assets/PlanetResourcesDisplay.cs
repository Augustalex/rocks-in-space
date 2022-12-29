using TMPro;
using UnityEngine;

public class PlanetResourcesDisplay : Hidable
{
    private TMP_Text _textComponent;
    private CurrentPlanetController _currentPlanetController;

    void Start()
    {
        _currentPlanetController = CurrentPlanetController.Get();
        _textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        var currentPlanet = _currentPlanetController.CurrentPlanet();
        
        if (!currentPlanet || currentPlanet.Anonymous())
        {
            _textComponent.text = "";
        }
        else
        {
            ShowPlanetResources(currentPlanet.GetResources());
        }
    }

    private void ShowPlanetResources(TinyPlanetResources currentPlanet)
    { 
        var energy = Mathf.RoundToInt(currentPlanet.GetEnergy());
        var food = Mathf.RoundToInt(currentPlanet.GetFood());
        var vacantHousing = currentPlanet.GetVacantHousing();
        var inhabitants = currentPlanet.GetInhabitants();
        _textComponent.text =
            $"Energy: {energy}  Food: {food}  Housing: {vacantHousing}  Inhabitants: {inhabitants}";
    }
}
