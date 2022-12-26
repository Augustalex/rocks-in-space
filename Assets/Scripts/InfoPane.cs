using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoPane : MonoBehaviour
{
    private TMP_Text _header;

    private void Awake()
    {
        _header = GetComponentInChildren<TMP_Text>();
    }
    
    public void Show(string headerText)
    {
        UpdatePosition();

        _header.text = headerText;
    }

    public void Hide()
    {
        _header.text = "";
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = Input.mousePosition;
    }
}
