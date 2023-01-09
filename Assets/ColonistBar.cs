using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColonistBar : MonoBehaviour
{
    [Header("Dead settings")] public Texture dead;
    public Color deadColor;

    [Header("Sad settings")] public Texture sad;
    public Color sadColor;

    [Header("Neutral settings")] public Texture neutral;
    public Color neutralColor;

    [Header("Happy settings")] public Texture happy;
    public Color happyColor;

    [Header("Overjoyed settings")] public Texture overjoyed;
    public Color overjoyedColor;

    [Header("References")] public TMP_Text text;
    public RawImage icon;

    void Update()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet == null) return;

        var resources = currentPlanet.GetResources();
        var inhabitants = resources.GetInhabitants();

        text.text = inhabitants == 0 ? "Uninhabited" : $"{inhabitants} colonists";

        var monitor = currentPlanet.GetColonistMonitor();
        var status = monitor.GetPlanetStatus();
        if (resources.HasHadDeaths() && status == PlanetColonistMonitor.PlanetStatus.Uninhabited)
        {
            SetIcon(dead, deadColor);
        }
        else if (status == PlanetColonistMonitor.PlanetStatus.Uninhabited)
        {
            icon.gameObject.SetActive(false);
        }
        else if (status == PlanetColonistMonitor.PlanetStatus.MovingOut)
        {
            SetIcon(sad, sadColor);
        }
        else if (status == PlanetColonistMonitor.PlanetStatus.Neutral)
        {
            SetIcon(neutral, neutralColor);
        }
        else if (status == PlanetColonistMonitor.PlanetStatus.Happy)
        {
            SetIcon(happy, happyColor);
        }
        else if (status == PlanetColonistMonitor.PlanetStatus.Overjoyed)
        {
            SetIcon(overjoyed, overjoyedColor);
        }
        else
        {
            throw new ArgumentOutOfRangeException();
        }
    }

    private void SetIcon(Texture texture, Color color)
    {
        icon.gameObject.SetActive(true);
        icon.texture = texture;
        icon.color = color;
    }
}