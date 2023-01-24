using System;
using UI.Tooltip;
using UnityEngine;

[RequireComponent(typeof(TooltipTriggerEvent))]
public class ColonistMeterTooltip : MonoBehaviour
{
    void Awake()
    {
        var trigger = GetComponent<TooltipTriggerEvent>();
        trigger.Triggered += TriggerTooltip;
    }

    private void TriggerTooltip()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (!currentPlanet) return;

        var colonistMonitor = currentPlanet.GetColonistMonitor();
        var planetStatus = colonistMonitor.GetPlanetStatus();

        var message = planetStatus switch
        {
            PlanetColonistMonitor.PlanetStatus.Uninhabited => "There are no colonists here.",
            PlanetColonistMonitor.PlanetStatus.MovingOut => "Colonists are Dying from No Protein and No Power!",
            PlanetColonistMonitor.PlanetStatus.Surviving => "Colonists are Surviving but lack Protein.",
            PlanetColonistMonitor.PlanetStatus.Neutral =>
                "Colonists are neither Happy nor Discontent with their life style.",
            PlanetColonistMonitor.PlanetStatus.Happy =>
                "Colonists are Happy but long for fresh food and a good drink.",
            PlanetColonistMonitor.PlanetStatus.Overjoyed => "Colonists are Overjoyed from their luxurious life style!",
            _ => ""
        };
        GlobalTooltip.Get().Show(message, Input.mousePosition);
    }
}