using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapPopupResourcesSection : MonoBehaviour
{
    private TMP_Text[] _texts;

    void Awake()
    {
        _texts =
            GetComponentsInChildren<TMP_Text>();
    }

    public void UpdateTexts(TinyPlanet planet)
    {
        _texts[0].text = planet.planetName;
        var tinyPlanetResources = planet.GetResources();
        _texts[1].text = $"Ore: {tinyPlanetResources.GetOre()}";
        _texts[2].text = $"Metals: {tinyPlanetResources.GetMetals()}";
        _texts[3].text = $"Gadgets: {tinyPlanetResources.GetGadgets()}";
    }
}
