using UnityEngine;

[RequireComponent(typeof(BuildingDescription))]
[RequireComponent(typeof(ModuleController))]
public class HousingBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get()
    {
        var controller = GetComponent<ModuleController>();

        var houseText = TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Housing);
        var energyText = TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy);
        var foodText = TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Food);

        return
            $"{StringUtils.Capitalized(houseText)} {TinyPlanetResources.InhabitantsPerResidency} colonists & generate {controller.cashPerMinute * 2}<sprite name=\"coin\"> every minute in income. Require {energyText} & {foodText}.";
    }
}