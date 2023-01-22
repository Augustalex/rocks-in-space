using System;
using UnityEngine;

public class SelectResourceController : MonoBehaviour
{
    // public TwoStateButton oreButton;
    public TwoStateButton ironButton;
    public TwoStateButton graphiteButton;
    public TwoStateButton copperButton;
    public TwoStateButton metalsButton;
    public TwoStateButton gadgetsButton;
    public TwoStateButton waterButton;
    public TwoStateButton refreshmentsButton;

    private TinyPlanetResources.PlanetResourceType _selectedResourceType;

    public event Action<TinyPlanetResources.PlanetResourceType> ResourceSelected;

    void Start()
    {
        // oreButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Ore);
        // oreButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Ore));

        ironButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Iron);
        ironButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Iron));

        graphiteButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Graphite);
        graphiteButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Graphite));

        copperButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Copper);
        copperButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Copper));

        metalsButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Metals);
        metalsButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Metals));

        gadgetsButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Gadgets);
        gadgetsButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets));

        waterButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Water);
        waterButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Water));

        refreshmentsButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Refreshments);
        refreshmentsButton.SetText(
            TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Refreshments));
    }

    public void SetSelectedResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        SelectResource(resourceType);
    }

    private void ResourceButtonClicked(TinyPlanetResources.PlanetResourceType resourceType)
    {
        SelectResource(resourceType);
        ResourceSelected?.Invoke(_selectedResourceType);
    }

    private void SelectResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        _selectedResourceType = resourceType;
        Render();
    }

    private void Render()
    {
        // oreButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Ore);
        ironButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Iron);
        graphiteButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Graphite);
        copperButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Copper);
        metalsButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Metals);
        gadgetsButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Gadgets);
        waterButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Water);
        refreshmentsButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Refreshments);
    }
}