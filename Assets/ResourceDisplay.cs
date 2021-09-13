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

        _text =
            $"Ore: ${currentPlanet.ore} - Metals: ${currentPlanet.metals} - Gadgets: ${currentPlanet.gadgets} - Energy: ${currentPlanet.energy}";
    }

    public void Hidden()
    {
        _hidden = true;
    }
}
