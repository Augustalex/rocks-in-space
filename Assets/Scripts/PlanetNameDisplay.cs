using TMPro;

public class PlanetNameDisplay : Hidable
{
    public string text;
    private TMP_Text _text;

    void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        _text.text = text;
    }
}