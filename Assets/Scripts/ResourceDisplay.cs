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
        var cash = Math.Floor(GlobalResources.Get().GetCash());
        _text =
            $"Credits: {(int)cash}c";
    }
}