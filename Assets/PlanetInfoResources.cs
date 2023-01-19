using Interactors;
using UnityEngine;

public class PlanetInfoResources : MonoBehaviour
{
    public GameObject basicResources;
    public GameObject iceResources;
    public GameObject colonyResources;

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
        food.Set(TinyPlanetResources.PlanetResourceType.Food);
        housing.Set(TinyPlanetResources.PlanetResourceType.Housing);

        ice.Set(TinyPlanetResources.PlanetResourceType.Ice);
        water.Set(TinyPlanetResources.PlanetResourceType.Water);
        refreshment.Set(TinyPlanetResources.PlanetResourceType.Refreshments);

        ore.Set(TinyPlanetResources.PlanetResourceType.Ore);
        metals.Set(TinyPlanetResources.PlanetResourceType.Metals);
        gadgets.Set(TinyPlanetResources.PlanetResourceType.Gadgets);

        colonyResources.SetActive(false);
    }

    void Update()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet != null)
        {
            var resources = currentPlanet.GetResources();

            var hasAnyIceRelatedItems = resources.HasBuilding(BuildingType.Purifier) || resources.HasBuilding(BuildingType.Distillery);
            var hasAnyIceResources = resources.GetResource(TinyPlanetResources.PlanetResourceType.Ice) > 0 ||
                                     resources.GetResource(TinyPlanetResources.PlanetResourceType.Water) > 0 ||
                                     resources.GetResource(TinyPlanetResources.PlanetResourceType.Refreshments) > 0;
            var iceMenuActive = hasAnyIceResources ||
                                hasAnyIceRelatedItems ||
                                currentPlanet.IsIcePlanet();

            var hasAnyHousingRelatedItems = resources.HasBuilding(BuildingType.FarmDome) || resources.HasBuilding(BuildingType.PowerPlant) ||
                                            resources.HasVacancy();

            if (hasAnyHousingRelatedItems || hasAnyIceRelatedItems)
            {
                colonyResources.SetActive(true);
                power.Refresh(Mathf.FloorToInt(resources.GetEnergy()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Energy));
                food.Refresh(Mathf.FloorToInt(resources.GetFood()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Food));
                housing.Refresh(Mathf.FloorToInt(resources.GetVacantHousing()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Housing));
            }
            else
            {
                colonyResources.SetActive(false);
            }

            if (iceMenuActive)
            {
                iceResources.SetActive(true);
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
                iceResources.SetActive(false);
            }

            var hasAnyBasicItems = resources.HasBuilding(BuildingType.Refinery) ||
                                   resources.HasBuilding(BuildingType.Factory);
            var hasAnyBasicResources = resources.GetResource(TinyPlanetResources.PlanetResourceType.Ore) > 0 ||
                                       resources.GetResource(TinyPlanetResources.PlanetResourceType.Metals) > 0 ||
                                       resources.GetResource(TinyPlanetResources.PlanetResourceType.Gadgets) > 0;

            if (iceMenuActive && !hasAnyBasicItems && !hasAnyIceRelatedItems && !hasAnyHousingRelatedItems && !hasAnyBasicResources)
            {
                basicResources.SetActive(false);
            }
            else
            {
                basicResources.SetActive(true);

                ore.Refresh(Mathf.FloorToInt(resources.GetOre()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ore));
                metals.Refresh(Mathf.FloorToInt(resources.GetMetals()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Metals));
                gadgets.Refresh(Mathf.FloorToInt(resources.GetGadgets()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Gadgets));
            }
        }
    }
}