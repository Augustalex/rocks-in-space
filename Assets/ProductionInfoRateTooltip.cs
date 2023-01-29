using Interactors;
using UI.Tooltip;
using UnityEngine;

[RequireComponent(typeof(TooltipTriggerEvent))]
public class ProductionInfoRateTooltip : MonoBehaviour
{
    private BuildingType _buildingType;

    void Awake()
    {
        var trigger = GetComponent<TooltipTriggerEvent>();
        trigger.Triggered += TriggerTooltip;
    }

    private void TriggerTooltip()
    {
        if (_buildingType == BuildingType.PowerPlant || _buildingType == BuildingType.SolarPanels)
        {
            GlobalTooltip.Get().Show("Energy output", Input.mousePosition);
        }
        else
        {
            GlobalTooltip.Get().Show("Production rate", Input.mousePosition);
        }
    }

    public void SetBuildingType(BuildingType buildingType)
    {
        _buildingType = buildingType;
    }
}