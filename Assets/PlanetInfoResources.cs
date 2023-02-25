using System;
using System.Collections;
using Interactors;
using UnityEngine;

public class PlanetInfoResources : MonoBehaviour
{
    public PlanetInfoResourceController ore;
    public PlanetInfoResourceController iron;
    public PlanetInfoResourceController graphite;
    public PlanetInfoResourceController copper;

    public PlanetInfoResourceController metals;
    public PlanetInfoResourceController gadgets;

    public PlanetInfoResourceController ice;
    public PlanetInfoResourceController water;
    public PlanetInfoResourceController refreshment;

    public PlanetInfoResourceController power;
    public PlanetInfoResourceController protein;
    public PlanetInfoResourceController food;
    public PlanetInfoResourceController housing;

    void Start()
    {
        power.Set(TinyPlanetResources.PlanetResourceType.Energy);
        power.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        power.GetComponent<TooltipTrigger>().SetMessage("Power");

        food.Set(TinyPlanetResources.PlanetResourceType.Food);
        food.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        food.GetComponent<TooltipTrigger>().SetMessage("Fresh food");

        housing.Set(TinyPlanetResources.PlanetResourceType.Housing);
        housing.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        housing.GetComponent<TooltipTrigger>().SetMessage("Vacant housing");

        ice.Set(TinyPlanetResources.PlanetResourceType.Ice);
        ice.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        ice.GetComponent<TooltipTrigger>().SetMessage("Ice");

        water.Set(TinyPlanetResources.PlanetResourceType.Water);
        water.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        water.GetComponent<TooltipTrigger>().SetMessage("Water");

        refreshment.Set(TinyPlanetResources.PlanetResourceType.Refreshments);
        refreshment.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        refreshment.GetComponent<TooltipTrigger>().SetMessage("Drinks");

        ore.gameObject.SetActive(false);
        // ore.Set(TinyPlanetResources.PlanetResourceType.Ore);
        // ore.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);

        metals.Set(TinyPlanetResources.PlanetResourceType.Metals);
        metals.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        metals.GetComponent<TooltipTrigger>().SetMessage("Metals");

        gadgets.Set(TinyPlanetResources.PlanetResourceType.Gadgets);
        gadgets.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        gadgets.GetComponent<TooltipTrigger>().SetMessage("Gadgets");

        iron.Set(TinyPlanetResources.PlanetResourceType.IronOre);
        iron.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        iron.GetComponent<TooltipTrigger>().SetMessage("Iron");

        graphite.Set(TinyPlanetResources.PlanetResourceType.Graphite);
        graphite.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        graphite.GetComponent<TooltipTrigger>().SetMessage("Graphite");

        copper.Set(TinyPlanetResources.PlanetResourceType.Copper);
        copper.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        copper.GetComponent<TooltipTrigger>().SetMessage("Copper");

        protein.Set(TinyPlanetResources.PlanetResourceType.Protein);
        protein.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        protein.GetComponent<TooltipTrigger>().SetMessage("Protein chunks");

        DisableColonyResources();

        StartCoroutine(StartUpdateLoop());
    }

    private void Update()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet != null)
        {
            var resources = currentPlanet.GetResources();

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

            if (resources.GetVacantHousing() > 0 || resources.HasBuilding(BuildingType.ResidentModule))
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

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Ice) > 0 ||
                resources.HasBuilding(BuildingType.Purifier))
            {
                ice.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Ice)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ice));
                ice.gameObject.SetActive(true);
            }
            else
            {
                ice.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.IronOre) > 0 ||
                resources.HasBuilding(BuildingType.Refinery))
            {
                iron.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.IronOre)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.IronOre));
                iron.gameObject.SetActive(true);
            }
            else
            {
                iron.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Graphite) > 0 ||
                resources.HasBuilding(BuildingType.Refinery))
            {
                graphite.Refresh(
                    Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Graphite)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Graphite));
                graphite.gameObject.SetActive(true);
            }
            else
            {
                graphite.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Copper) > 0 ||
                resources.HasBuilding(BuildingType.Factory))
            {
                copper.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Copper)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Copper));
                copper.gameObject.SetActive(true);
            }
            else
            {
                copper.gameObject.SetActive(false);
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

    IEnumerator StartUpdateLoop()
    {
        while (gameObject != null)
        {
            UpdateGeneratedResources();
            yield return new WaitForSeconds(1f);
        }
    }

    void UpdateGeneratedResources()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet != null)
        {
            var resources = currentPlanet.GetResources();

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Food) > 0 ||
                resources.HasBuilding(BuildingType.FarmDome))
            {
                food.Refresh(Mathf.FloorToInt(resources.GetFood()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Food));
                food.gameObject.SetActive(true);
            }
            else
            {
                food.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Water) > 0 ||
                resources.HasBuilding(BuildingType.Purifier) || resources.HasBuilding(BuildingType.Distillery))
            {
                water.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Water)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Water));
                water.gameObject.SetActive(true);
            }
            else
            {
                water.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Refreshments) > 0 ||
                resources.HasBuilding(BuildingType.Purifier) || resources.HasBuilding(BuildingType.Distillery))
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

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.Protein) > 0 ||
                resources.HasBuilding(BuildingType.ProteinFabricator))
            {
                protein.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.Protein)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.Protein));
                protein.gameObject.SetActive(true);
            }
            else
            {
                protein.gameObject.SetActive(false);
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