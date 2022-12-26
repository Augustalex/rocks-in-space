using TMPro;
using UnityEngine;

public class ColonistsCounter : MonoBehaviour
{
    private TMP_Text _text;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        var inhabitants =  TinyPlanetResources.GetGlobalInhabitants();
        if (inhabitants > 0)
        {
            _text.text = $"COLONISTS: {inhabitants}";
        }
        else
        {
            _text.text = "";
        }
    }
}
