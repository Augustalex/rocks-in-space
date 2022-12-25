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
        var energy = currentPlanet.GetEnergy();
        var food = currentPlanet.GetFood();
        var inhabitants = currentPlanet.GetInhabitants();
        _text =
            $"Inhabitants: {inhabitants}\nOre: {ore}\nMetals: {metals}\nGadgets: {gadgets}\nEnergy: {energy}\nFood: {food}\n";
            // $"Ore: {ore} -> Metals: {metals} -> Gadgets: {gadgets} -> Energy: {energy} -> Food: {food} -> Inhabitants: {inhabitants}";
            // _text = $"ORE: {ore}";
    }

    public void Hidden()
    {
        _hidden = true;
    }
}
