using System;
using Interactors;
using UnityEngine;

public class SelectResourceController : MonoBehaviour
{
    public TwoStateButton oreButton;
    public TwoStateButton metalsButton;
    public TwoStateButton gadgetsButton;

    private TinyPlanetResources.PlanetResourceType _selectedResourceType;

    public event Action<TinyPlanetResources.PlanetResourceType> ResourceSelected;

    void Start()
    {
        oreButton.Clicked += OreButtonClicked;
        oreButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Ore));

        metalsButton.Clicked += MetalsButtonClicked;
        metalsButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Metals));

        gadgetsButton.Clicked += GadgetsButtonClicked;
        gadgetsButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets));
    }

    public void SetSelectedResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        switch (resourceType)
        {
            case TinyPlanetResources.PlanetResourceType.Metals:
                SelectMetals();
                break;
            case TinyPlanetResources.PlanetResourceType.Gadgets:
                SelectGadgets();
                break;
            default:
                SelectOre();
                break;
        }
    }

    private void OreButtonClicked()
    {
        SelectOre();
        ResourceSelected?.Invoke(_selectedResourceType);
    }

    private void MetalsButtonClicked()
    {
        SelectMetals();
        ResourceSelected?.Invoke(_selectedResourceType);
    }

    private void GadgetsButtonClicked()
    {
        SelectGadgets();
        ResourceSelected?.Invoke(_selectedResourceType);
    }

    private void SelectOre()
    {
        _selectedResourceType = TinyPlanetResources.PlanetResourceType.Ore;
        Render();
    }

    private void SelectMetals()
    {
        _selectedResourceType = TinyPlanetResources.PlanetResourceType.Metals;
        Render();
    }

    private void SelectGadgets()
    {
        _selectedResourceType = TinyPlanetResources.PlanetResourceType.Gadgets;
        Render();
    }

    private void Render()
    {
        oreButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Ore);
        metalsButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Metals);
        gadgetsButton.Set(_selectedResourceType == TinyPlanetResources.PlanetResourceType.Gadgets);
    }

    void Update()
    {
    }
}