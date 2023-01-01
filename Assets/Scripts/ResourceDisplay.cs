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
            var globalResources = currentPlanet.GetResources();
            var ore = globalResources.GetOre();
            var metals = globalResources.GetMetals();
            var gadgets = globalResources.GetGadgets();
            var cash = GlobalResources.Get().GetCash();
            _text =
                $"Credits: {cash}c\nOre: {ore}\nMetals: {metals}\nGadgets: {gadgets}";   
        }
    }
}
