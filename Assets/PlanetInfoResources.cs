using TMPro;
using UnityEngine;

public class PlanetInfoResources : MonoBehaviour
{
    public GameObject colonyResources;

    public TMP_Text ore;
    public TMP_Text metals;
    public TMP_Text gadgets;
    public TMP_Text power;
    public TMP_Text food;
    public TMP_Text housing;

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
                power.text = resources.GetEnergy().ToString();
                food.text = resources.GetFood().ToString();
                housing.text = resources.GetVacantHousing().ToString();
            }
            else
            {
                colonyResources.SetActive(false);
            }

            ore.text = resources.GetOre().ToString();
            metals.text = resources.GetMetals().ToString();
            gadgets.text = resources.GetGadgets().ToString();
        }
    }
}