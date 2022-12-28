using TMPro;
using UnityEngine;

public class ColonistsCounter : MonoBehaviour
{
    private TMP_Text _text;
    private CurrentPlanetController _currentPlanetController;

    void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        _currentPlanetController = CurrentPlanetController.Get();
    }

    void Update()
    {
        var resources = _currentPlanetController.CurrentPlanet().GetResources();
        var inhabitants = resources.GetInhabitants();
        _text.text = inhabitants > 0 ? $"COLONISTS: {inhabitants}" : "";
    }
}