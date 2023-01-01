using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapPopupNameSection : MonoBehaviour
{
    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void UpdateTexts(TinyPlanet planet)
    {
        _text.text = planet.planetName;
    }
}
