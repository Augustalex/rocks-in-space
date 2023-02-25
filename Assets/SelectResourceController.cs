using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectResourceController : MonoBehaviour
{
    public TwoStateButton ironButton;
    public TwoStateButton graphiteButton;
    public TwoStateButton copperButton;
    [FormerlySerializedAs("metalsButton")] public TwoStateButton ironPlatesButton;
    public TwoStateButton copperPlatesButton;
    public TwoStateButton gadgetsButton;
    public TwoStateButton waterButton;
    public TwoStateButton refreshmentsButton;

    private TinyPlanetResources.PlanetResourceType _selectedResourceType;

    public event Action<TinyPlanetResources.PlanetResourceType> ResourceSelected;

    void Start()
    {
        ProgressManager.Get().OnResourceGot += (_) => Render();

        ironButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.IronOre);
        ironButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.IronOre));

        graphiteButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.Graphite);
        graphiteButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Graphite));

        copperButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.CopperOre);
        copperButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.CopperOre));

        ironPlatesButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.IronPlates);
        ironPlatesButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.IronPlates));

        copperPlatesButton.Clicked += () => ResourceButtonClicked(TinyPlanetResources.PlanetResourceType.CopperPlates);
        copperPlatesButton.SetText(TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.CopperPlates));

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
        var progressManager = ProgressManager.Get();
        RenderButton(
            ironButton,
            true,
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.IronOre
        );

        RenderButton(
            graphiteButton,
            true,
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.Graphite
        );

        RenderButton(
            copperButton,
            true,
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.CopperOre
        );

        RenderButton(
            ironPlatesButton,
            progressManager.GotResource(TinyPlanetResources.PlanetResourceType.IronPlates),
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.IronPlates
        );

        RenderButton(
            copperPlatesButton,
            progressManager.GotResource(TinyPlanetResources.PlanetResourceType.CopperPlates),
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.CopperPlates
        );

        RenderButton(
            gadgetsButton,
            progressManager.GotResource(TinyPlanetResources.PlanetResourceType.Gadgets),
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.Gadgets
        );

        RenderButton(
            waterButton,
            progressManager.GotResource(TinyPlanetResources.PlanetResourceType.Water),
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.Water
        );

        RenderButton(
            refreshmentsButton,
            progressManager.GotResource(TinyPlanetResources.PlanetResourceType.Refreshments),
            _selectedResourceType == TinyPlanetResources.PlanetResourceType.Refreshments
        );
    }

    private void RenderButton(TwoStateButton button, bool shouldRender, bool shouldSelect)
    {
        if (!shouldRender) button.Hide();
        else
        {
            button.Show();
            button.Set(shouldSelect);
        }
    }
}