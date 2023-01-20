using System;
using UnityEngine;

[RequireComponent(typeof(BuildingDescription))]
[RequireComponent(typeof(FarmController))]
public class FarmsBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var controller = GetComponent<FarmController>();
        var foodText = TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Food);
        return
            $"Produces {controller.foodPerMinute} {foodText} every minute. {StringUtils.Capitalized(foodText)} is consumed by colonists.";
    }
}