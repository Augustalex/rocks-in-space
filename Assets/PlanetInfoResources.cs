using Interactors;
using UnityEngine;

public class PlanetInfoResources : MonoBehaviour
{
    public PlanetInfoResourceController ore;
    public PlanetInfoResourceController metals;
    public PlanetInfoResourceController gadgets;

    public PlanetInfoResourceController ice;
    public PlanetInfoResourceController water;
    public PlanetInfoResourceController refreshment;

    public PlanetInfoResourceController power;
    public PlanetInfoResourceController food;
    public PlanetInfoResourceController housing;

    void Start()
    {
        power.Set(TinyPlanetResources.PlanetResourceType.Energy);
        power.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        food.Set(TinyPlanetResources.PlanetResourceType.Food);
        food.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        housing.Set(TinyPlanetResources.PlanetResourceType.Housing);
        housing.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        ice.Set(TinyPlanetResources.PlanetResourceType.Ice);
        ice.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        water.Set(TinyPlanetResources.PlanetResourceType.Water);
        water.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        refreshment.Set(TinyPlanetResources.PlanetResourceType.Refreshments);
        refreshment.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        ore.Set(TinyPlanetResources.PlanetResourceType.Ore);
        ore.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        metals.Set(TinyPlanetResources.PlanetResourceType.Metals);
        metals.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        gadgets.Set(TinyPlanetResources.PlanetResourceType.Gadgets);
        gadgets.Refresh(0, TinyPlanetResources.ResourceTrend.neutral);

        DisableColonyResources();
    }

    void Update()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet != null)
        {
            var resources = currentPlanet.GetResources();

            var hasAnyIceRelatedItems = resources.HasBuilding(BuildingType.Purifier) ||
                                        resources.HasBuilding(BuildingType.Distillery);
            var hasAnyIceResources = resources.GetResource(TinyPlanetResources.PlanetResourceType.Ice) > 0 ||
                                     resources.GetResource(TinyPlanetResources.PlanetResourceType.Water) > 0 ||
                                     resources.GetResource(TinyPlanetResources.PlanetResourceType.Refreshments) > 0;
            var iceMenuActive = hasAnyIceResources ||
                                hasAnyIceRelatedItems ||
                                currentPlanet.IsIcePlanet();

            var hasAnyHousingRelatedItems = resources.HasBuilding(BuildingType.FarmDome) ||
                                            resources.HasBuilding(BuildingType.PowerPlant) ||
                                            resources.HasVacancy();

            if (hasAnyHousingRelatedItems || hasAnyIceRelatedItems)
            {
                power.gameObject.SetActive(true);
                food.gameObject.SetActive(true);
                housing.gameObject.SetActive(true);

                power.Refresh(Mathf.FloorToInt(resources.GetEnergy()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Energy));
                food.Refresh(Mathf.FloorToInt(resources.GetFood()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Food));
                housing.Refresh(Mathf.FloorToInt(resources.GetVacantHousing()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Housing));
            }
            else
            {
                DisableColonyResources();
            }

            if (iceMenuActive)
            {
                ice.gameObject.SetActive(true);
                water.gameObject.SetActive(true);
                refreshment.gameObject.SetActive(true);

                ice.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Ice)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ice));
                water.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Water)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Water));
                refreshment.Refresh(
                    Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Refreshments)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Refreshments));
            }
            else
            {
                ice.gameObject.SetActive(false);
                water.gameObject.SetActive(false);
                refreshment.gameObject.SetActive(false);
            }

            var hasAnyBasicItems = resources.HasBuilding(BuildingType.Refinery) ||
                                   resources.HasBuilding(BuildingType.Factory);
            var hasAnyBasicResources = resources.GetResource(TinyPlanetResources.PlanetResourceType.Ore) > 0 ||
                                       resources.GetResource(TinyPlanetResources.PlanetResourceType.Metals) > 0 ||
                                       resources.GetResource(TinyPlanetResources.PlanetResourceType.Gadgets) > 0;

            if (iceMenuActive && !hasAnyBasicItems && !hasAnyIceRelatedItems && !hasAnyHousingRelatedItems &&
                !hasAnyBasicResources)
            {
                ore.gameObject.SetActive(false);
                metals.gameObject.SetActive(false);
                gadgets.gameObject.SetActive(false);
            }
            else
            {
                ore.gameObject.SetActive(true);
                metals.gameObject.SetActive(true);
                gadgets.gameObject.SetActive(true);

                ore.Refresh(Mathf.FloorToInt(resources.GetOre()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ore));
                metals.Refresh(Mathf.FloorToInt(resources.GetMetals()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Metals));
                gadgets.Refresh(Mathf.FloorToInt(resources.GetGadgets()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Gadgets));
            }
        }
    }

    private void DisableColonyResources()
    {
        power.gameObject.SetActive(false);
        food.gameObject.SetActive(false);
        housing.gameObject.SetActive(false);
    }
}