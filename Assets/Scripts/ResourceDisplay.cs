using System;
using TMPro;

public class ResourceDisplay : Hidable
{
    private string _text;
    private TMP_Text _textComponent;

    void Start()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        ShowGlobalResources();
        _textComponent.text = _text;
    }

    public void ShowGlobalResources()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (!currentPlanet) _text = "";
        else
        {
            var cash = Math.Round(GlobalResources.Get().GetCash());
            _text =
                $"Credits: {(int)cash}c";
        }
    }
}