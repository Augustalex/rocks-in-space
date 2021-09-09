using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetNameDisplay : MonoBehaviour
{
    public bool hidden = false;
    public string text;
    private TMP_Text _text;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        if (hidden)
        {
            _text.text = "";
        }
        else
        {
            _text.text = text;
        }
    }
}