using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptyCargoSlotIcons : MonoBehaviour
{
    public GameObject iconTemplate;

    public event Action<TinyPlanetResources.PlanetResourceType> ResourceSelected;

    private readonly TinyPlanetResources.PlanetResourceType[] _resources =
    {
        TinyPlanetResources.PlanetResourceType.Graphite,
        TinyPlanetResources.PlanetResourceType.IronOre,
        TinyPlanetResources.PlanetResourceType.CopperOre,
        TinyPlanetResources.PlanetResourceType.IronPlates,
        TinyPlanetResources.PlanetResourceType.CopperPlates,
        TinyPlanetResources.PlanetResourceType.Gadgets,
        TinyPlanetResources.PlanetResourceType.Ice,
        TinyPlanetResources.PlanetResourceType.Water,
        TinyPlanetResources.PlanetResourceType.Refreshments,
        TinyPlanetResources.PlanetResourceType.Protein,
        TinyPlanetResources.PlanetResourceType.Food,
    };

    private GameObject[] _icons = { };

    public void UpdateIcons()
    {
        var planet = CurrentPlanetController.Get().CurrentPlanet();
        if (!planet) return;

        foreach (var icon in _icons)
        {
            Destroy(icon);
        }

        var resources = planet.GetResources();

        var newIcons = new List<GameObject>();
        foreach (var resource in _resources)
        {
            if (resources.GetResource(resource) > 0)
            {
                var icon = Instantiate(iconTemplate, transform, true);
                var button = icon.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    if (resources.GetResource(resource) > 0)
                    {
                        SelectResource(resource);
                    }
                });
                var resourceTexture = (Texture2D)UIAssetManager.Get().GetResourceTexture(resource);
                var resourceSprite = Sprite.Create(resourceTexture,
                    new Rect(0, 0, resourceTexture.width, resourceTexture.height), new Vector2(0.5f, 0.5f));
                button.GetComponent<Image>().sprite = resourceSprite;

                newIcons.Add(icon);
            }
        }

        _icons = newIcons.ToArray();
    }

    private void SelectResource(TinyPlanetResources.PlanetResourceType resource)
    {
        ResourceSelected?.Invoke(resource);
    }
}