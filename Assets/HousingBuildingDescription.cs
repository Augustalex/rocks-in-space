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
        var refreshmentsText = TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Refreshments);

        return
            $"{StringUtils.Capitalized(houseText)} for {TinyPlanetResources.InhabitantsPerResidency} colonists. Provides {controller.cashPerMinute}<sprite name=\"coin\">/min for {energyText}\nx2 for {foodText} x4 for {refreshmentsText}.";
    }
}