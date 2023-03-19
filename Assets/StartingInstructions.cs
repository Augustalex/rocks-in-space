using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartingInstructions : MonoBehaviour
{
    private static StartingInstructions _instance;
    private TMP_Text _text;

    public static StartingInstructions Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;

        _text = GetComponent<TMP_Text>();
        _text.text = "";
        _text.gameObject.SetActive(false);
    }

    public void Print(string text)
    {
        _text.gameObject.SetActive(true);
        _text.text = text;
    }

    public void Clear()
    {
        _text.gameObject.SetActive(false);
    }
}