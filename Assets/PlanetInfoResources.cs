using UnityEngine;

public class PlanetInfoResources : MonoBehaviour
{
    public GameObject colonyResources;

    public PlanetInfoResourceController ore;
    public PlanetInfoResourceController metals;
    public PlanetInfoResourceController gadgets;
    public PlanetInfoResourceController power;
    public PlanetInfoResourceController food;
    public PlanetInfoResourceController housing;

    void Start()
    {
        power.Set(TinyPlanetResources.PlanetResourceType.Energy);
        food.Set(TinyPlanetResources.PlanetResourceType.Food);
        housing.Set(TinyPlanetResources.PlanetResourceType.Housing);

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
            if (resources.HasFarm() || resources.HasPowerPlant() ||
                resources.HasVacancy())
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

            ore.Refresh(Mathf.FloorToInt(resources.GetOre()),
                resources.GetTrend(TinyPlanetResources.PlanetResourceType.Ore));
            metals.Refresh(Mathf.FloorToInt(resources.GetMetals()),
                resources.GetTrend(TinyPlanetResources.PlanetResourceType.Metals));
            gadgets.Refresh(Mathf.FloorToInt(resources.GetGadgets()),
                resources.GetTrend(TinyPlanetResources.PlanetResourceType.Gadgets));
        }
    }
}