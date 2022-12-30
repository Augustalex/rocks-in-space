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
        var globalResources = GlobalResources.Get();
        var ore = globalResources.GetOre();
        var metals = globalResources.GetMetals();
        var gadgets = globalResources.GetGadgets();
        var cash = globalResources.GetCash();
        _text =
            $"Credits: {cash}c\nOre: {ore}\nMetals: {metals}\nGadgets: {gadgets}";
    }
}
