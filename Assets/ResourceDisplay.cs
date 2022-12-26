using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceDisplay : MonoBehaviour
{
    private string _text;
    private TMP_Text _textComponent;
    private bool _hidden;

    void Start()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (_hidden)
        {
            _textComponent.text = "";
        }
        else
        {
            _textComponent.text = _text;
        }
    }

    public void NoPlanetSelected()
    {
        _hidden = false;

        _text = "No planet selected";
    }

    public void ShowPlanetResources(TinyPlanetResources currentPlanet)
    {
        _hidden = false;

        var ore = currentPlanet.GetOre();
        var metals = currentPlanet.GetMetals();
        var gadgets = currentPlanet.GetGadgets();
        var energy = Mathf.RoundToInt(currentPlanet.GetEnergy());
        var food = Mathf.RoundToInt(currentPlanet.GetFood());
        var vacantHousing = TinyPlanetResources.GetVacantHousing();
        _text =
            $"Ore: {ore}\nMetals: {metals}\nGadgets: {gadgets}\nEnergy: {energy}\nFood: {food}\nHousing: {vacantHousing}";
    }

    public void Hidden()
    {
        _hidden = true;
    }
}
