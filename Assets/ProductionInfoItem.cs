using System.Collections;
using Interactors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionInfoItem : MonoBehaviour
{
    public BuildingType buildingType;
    public TinyPlanetResources.PlanetResourceType resourceType;

    public RawImage icon;
    public TMP_Text quantityText;

    public TMP_Text productionRateText;
    public GameObject warningIcon;

    void Start()
    {
        UpdateIcons();

        CurrentPlanetController.Get().CurrentPlanetChanged += (_) => UpdateNow();

        UpdateNow();
        StartCoroutine(SlowUpdate());
    }

    private void UpdateIcons()
    {
        icon.texture = UIAssetManager.Get().GetBuildingIcon(buildingType);
    }

    IEnumerator SlowUpdate()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(1f);
            UpdateNow();
        }
    }

    private void UpdateNow()
    {
        var planet = CurrentPlanetController.Get().CurrentPlanet();
        if (!planet) return;

        var resources = planet.GetResources();
        var buildingCount = resources.GetBuildingCount(buildingType);
        quantityText.text = buildingCount.ToString().PadLeft(2, '0');
        var interactor = InteractorController.Get().GetGenericInteractorByBuildingType(buildingType);

        if (buildingType == BuildingType.ResidentModule)
        {
            var totalHousesCount = resources.GetTotalHousesCount();
            var occupied = resources.GetOccupantHousesCount();
            var occupancyRate = occupied / totalHousesCount;
            var occupancyText = Mathf.Round(occupancyRate * 1000f) / 10f;

            warningIcon.SetActive(false);
            productionRateText.gameObject.SetActive(true);

            var resourceTextIcon = TinyPlanetResources.ResourceSprite(resourceType);
            productionRateText.text = $"{resourceTextIcon}{occupancyText}%";
        }
        else if (buildingType == BuildingType.PowerPlant || buildingType == BuildingType.SolarPanels)
        {
            var resourceEffect = interactor.template.GetComponent<ResourceEffect>();

            warningIcon.SetActive(false);
            productionRateText.gameObject.SetActive(true);

            if (buildingType == BuildingType.PowerPlant)
            {
                var stoppedPowerPlants =
                    planet.GetProductionMonitor().GetStoppedBuildingsCount(BuildingType.PowerPlant);
                var workingPlants = buildingCount - stoppedPowerPlants;

                var energyOutput = resourceEffect.energy * workingPlants;
                var resourceTextIcon = TinyPlanetResources.ResourceSprite(resourceType);
                productionRateText.text = $"{resourceTextIcon}{energyOutput}/min";
            }
            else
            {
                var energyOutput = resourceEffect.energy * buildingCount;
                var resourceTextIcon = TinyPlanetResources.ResourceSprite(resourceType);
                productionRateText.text = $"{resourceTextIcon}{energyOutput}/min";
            }
        }
        else
        {
            var workerCount = resources.GetWorkers();
            var conversionEffect = interactor.template.GetComponent<ResourceConversionEffect>();

            var processTime = ((float)conversionEffect.iterationTime + ResourceConversionEffect.ResourceTakeTime);
            var actualProcessTime =
                workerCount >= 0 ? processTime : processTime * ResourceConversionEffect.SlowDownFactor;
            var productionPerMin = (float)conversionEffect.to / actualProcessTime;

            var stoppedBuildings = planet.GetProductionMonitor().GetStoppedBuildingsCount(buildingType);
            var workingBuildings = buildingCount - stoppedBuildings;

            var totalProductionPerSecond = workingBuildings * productionPerMin;
            var totalProductionPerMin = totalProductionPerSecond * 60f;

            if (totalProductionPerMin < 1)
            {
                warningIcon.SetActive(true);
                productionRateText.gameObject.SetActive(false);
            }
            else
            {
                warningIcon.SetActive(false);

                var resourceTextIcon = TinyPlanetResources.ResourceSprite(resourceType);
                var rounded = Mathf.Round(totalProductionPerMin / 10f) * 10f;
                productionRateText.text = $"{resourceTextIcon}{rounded}/min";
                productionRateText.gameObject.SetActive(true);
            }
        }
    }
}