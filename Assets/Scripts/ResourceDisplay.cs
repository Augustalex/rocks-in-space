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
        _text =
            $"Credits: {currentPlanet.GetCash()}c\nOre: {ore}\nMetals: {metals}\nGadgets: {gadgets}";
    }

    public void Hidden()
    {
        _hidden = true;
    }
}
