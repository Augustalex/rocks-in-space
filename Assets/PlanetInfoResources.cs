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
        power.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        food.Set(TinyPlanetResources.PlanetResourceType.Food);
        food.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        housing.Set(TinyPlanetResources.PlanetResourceType.Housing);
        housing.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        ice.Set(TinyPlanetResources.PlanetResourceType.Ice);
        ice.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        water.Set(TinyPlanetResources.PlanetResourceType.Water);
        water.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        refreshment.Set(TinyPlanetResources.PlanetResourceType.Refreshments);
        refreshment.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        ore.Set(TinyPlanetResources.PlanetResourceType.Ore);
        ore.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        metals.Set(TinyPlanetResources.PlanetResourceType.Metals);
        metals.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        gadgets.Set(TinyPlanetResources.PlanetResourceType.Gadgets);
        gadgets.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        DisableColonyResources();
    }

    void Update()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet != null)
        {
            var progressManager = ProgressManager.Get();

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
                                            resources.HasPowerBuilding() ||
                                            resources.HasVacantHousing();
            var shouldShowColonyResources = hasAnyHousingRelatedItems || hasAnyIceRelatedItems;

            if (resources.HasPowerBuilding() || resources.GetEnergy() < 0)
            {
                power.Refresh(Mathf.FloorToInt(resources.GetEnergy()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Energy));
                power.gameObject.SetActive(true);
            }
            else
            {
                power.gameObject.SetActive(false);
            }

            if (shouldShowColonyResources && progressManager.Comfortable())
            {
                food.Refresh(Mathf.FloorToInt(resources.GetFood()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Food));
                food.gameObject.SetActive(true);
            }
            else
            {
                food.gameObject.SetActive(false);
            }

            if (shouldShowColonyResources)
            {
                var vacantHousing = Mathf.FloorToInt(resources.GetVacantHousing());
                housing.Refresh(vacantHousing,
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Housing));
                housing.gameObject.SetActive(true);
            }
            else
            {
                housing.gameObject.SetActive(false);
            }

            if (iceMenuActive && progressManager.Surviving() && currentPlanet.IsIcePlanet())
            {
                ice.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Ice)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ice));
                ice.gameObject.SetActive(true);
            }
            else
            {
                ice.gameObject.SetActive(false);
            }

            if (iceMenuActive && progressManager.Surviving())
            {
                water.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Water)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Water));
                water.gameObject.SetActive(true);
            }
            else
            {
                water.gameObject.SetActive(false);
            }

            if (iceMenuActive && progressManager.Comfortable())
            {
                refreshment.Refresh(
                    Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Refreshments)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Refreshments));
                refreshment.gameObject.SetActive(true);
            }
            else
            {
                refreshment.gameObject.SetActive(false);
            }

            var hasAnyBasicItems = resources.HasBuilding(BuildingType.Refinery) ||
                                   resources.HasBuilding(BuildingType.Factory);
            var hasAnyBasicResources = resources.GetResource(TinyPlanetResources.PlanetResourceType.Ore) > 0 ||
                                       resources.GetResource(TinyPlanetResources.PlanetResourceType.Metals) > 0 ||
                                       resources.GetResource(TinyPlanetResources.PlanetResourceType.Gadgets) > 0;

            var showOnlyIceRelatedThings = iceMenuActive && !hasAnyBasicItems && !hasAnyIceRelatedItems &&
                                           !hasAnyHousingRelatedItems &&
                                           !hasAnyBasicResources;

            if (!showOnlyIceRelatedThings)
            {
                ore.Refresh(Mathf.FloorToInt(resources.GetOre()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ore));
                ore.gameObject.SetActive(true);
            }
            else
            {
                ore.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Metals) > 0 ||
                resources.HasBuilding(BuildingType.Refinery))
            {
                metals.Refresh(Mathf.FloorToInt(resources.GetMetals()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Metals));
                metals.gameObject.SetActive(true);
            }
            else
            {
                metals.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Gadgets) > 0 ||
                resources.HasBuilding(BuildingType.Factory))
            {
                gadgets.gameObject.SetActive(true);
                gadgets.Refresh(Mathf.FloorToInt(resources.GetGadgets()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Gadgets));
            }
            else
            {
                gadgets.gameObject.SetActive(false);
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