using System.Linq;
using TMPro;
using UnityEngine;

public class PlanetIncomeText : MonoBehaviour
{
    private TMP_Text _text;
    private TinyPlanet _planet;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    void Start()
    {
        CurrentPlanetController.Get().CurrentPlanetChanged += PlanetChanged;
    }

    private void PlanetChanged(PlanetChangedInfo info)
    {
        if (!info.NewPlanet) _planet = null;
        else _planet = info.NewPlanet;
    }

    void Update()
    {
        if (!_planet) return;

        var houseIncome = _planet.GetColonistMonitor().GetHouseIncomeEstimate();
        var cashEffect = _planet.GetCostMonitor().GetEstimatedPlanetCashEffect();
        var secondIncome = cashEffect + houseIncome;
        var minuteIncome = secondIncome * 60f;
        var incomeText = Mathf.RoundToInt(minuteIncome);
        _text.text = $"{incomeText}<sprite name=\"coin\">";
    }
}