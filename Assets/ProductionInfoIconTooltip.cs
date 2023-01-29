using Interactors;
using UI.Tooltip;
using UnityEngine;

[RequireComponent(typeof(TooltipTriggerEvent))]
public class ProductionInfoIconTooltip : MonoBehaviour
{
    private BuildingType _buildingType;

    void Awake()
    {
        var trigger = GetComponent<TooltipTriggerEvent>();
        trigger.Triggered += TriggerTooltip;
    }

    private void TriggerTooltip()
    {
        var buildingName = InteractorController.Get().GetGenericInteractorByBuildingType(_buildingType)
            .GetInteractorName();
        GlobalTooltip.Get().Show(buildingName, Input.mousePosition);
    }

    public void SetBuildingType(BuildingType buildingType)
    {
        _buildingType = buildingType;
    }
}