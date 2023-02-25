using System;
using System.Collections;
using Interactors;
using UnityEngine;
using UnityEngine.Serialization;

public class PlanetInfoResources : MonoBehaviour
{
    public PlanetInfoResourceController ore;
    [FormerlySerializedAs("iron")] public PlanetInfoResourceController ironOre;
    public PlanetInfoResourceController graphite;
    [FormerlySerializedAs("copper")] public PlanetInfoResourceController copperOre;

    [FormerlySerializedAs("metals")] public PlanetInfoResourceController ironPlates;
    public PlanetInfoResourceController copperPlates;
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

        ironOre.Set(TinyPlanetResources.PlanetResourceType.IronOre);
        ironOre.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        ironOre.GetComponent<TooltipTrigger>().SetMessage("Iron ore");

        ironPlates.Set(TinyPlanetResources.PlanetResourceType.IronPlates);
        ironPlates.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        ironPlates.GetComponent<TooltipTrigger>().SetMessage("Iron plates");

        copperOre.Set(TinyPlanetResources.PlanetResourceType.CopperOre);
        copperOre.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        copperOre.GetComponent<TooltipTrigger>().SetMessage("Copper ore");

        copperPlates.Set(TinyPlanetResources.PlanetResourceType.CopperPlates);
        copperPlates.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        copperPlates.GetComponent<TooltipTrigger>().SetMessage("Copper plates");
        
        gadgets.Set(TinyPlanetResources.PlanetResourceType.Gadgets);
        gadgets.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        gadgets.GetComponent<TooltipTrigger>().SetMessage("Gadgets");
        
        graphite.Set(TinyPlanetResources.PlanetResourceType.Graphite);
        graphite.Refresh(0, TinyPlanetResources.ResourceTrend.Neutral);
        graphite.GetComponent<TooltipTrigger>().SetMessage("Graphite");
        
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
                ironOre.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.IronOre)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.IronOre));
                ironOre.gameObject.SetActive(true);
            }
            else
            {
                ironOre.gameObject.SetActive(false);
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

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.CopperOre) > 0 ||
                resources.HasBuilding(BuildingType.Factory))
            {
                copperOre.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.CopperOre)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.CopperOre));
                copperOre.gameObject.SetActive(true);
            }
            else
            {
                copperOre.gameObject.SetActive(false);
            }

            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.CopperPlates) > 0 ||
                resources.HasBuilding(BuildingType.CopperRefinery))
            {
                copperPlates.Refresh(Mathf.FloorToInt(resources.GetResource(TinyPlanetResources.PlanetResourceType.CopperPlates)),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.CopperPlates));
                copperPlates.gameObject.SetActive(true);
            }
            else
            {
                copperPlates.gameObject.SetActive(false);
            }
            
            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.IronPlates) > 0 ||
                resources.HasBuilding(BuildingType.Refinery))
            {
                ironPlates.Refresh(Mathf.FloorToInt(resources.GetIronPlates()),
                    resources.GetTrend(TinyPlanetResources.PlanetResourceType.IronPlates));
                ironPlates.gameObject.SetActive(true);
            }
            else
            {
                ironPlates.gameObject.SetActive(false);
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