using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetTopMenu : MonoBehaviour
{
    private static PlanetTopMenu _instance;
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");
    private TinyPlanet _selectedPlanet;

    public TMP_Text headerText;
    public TMP_Text resourcesText;

    private readonly List<TinyPlanetResources.PlanetResourceType> _resources =
        new()
        {
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.Metals,
            TinyPlanetResources.PlanetResourceType.Gadgets,
            TinyPlanetResources.PlanetResourceType.Energy,
            TinyPlanetResources.PlanetResourceType.Food,
            TinyPlanetResources.PlanetResourceType.Housing,
            // TinyPlanetResources.PlanetResourceType.Inhabitants,
        };

    // When there are a lack of resources - show these as 0, in this order, to guide the player on what to do.
    private readonly List<TinyPlanetResources.PlanetResourceType> _tutorialItems =
        new()
        {
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.Metals,
            TinyPlanetResources.PlanetResourceType.Gadgets,
            // TinyPlanetResources.PlanetResourceType.Inhabitants,
        };

    public static PlanetTopMenu Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();

        _animator.SetBool(Visible, false);
    }

    private void Start()
    {
        CurrentPlanetController.Get().CurrentPlanetChanged += (_) => UpdateState();
        CurrentPlanetController.Get().ShipSelected += (_) => UpdateState();
        DisplayController.Get().OnRenameDone += UpdateState;
        CameraController.Get().OnToggleZoom += (_) => UpdateState();
    }

    private void Update()
    {
        if (HiddenAlready()) return;
        if (_selectedPlanet == null) return;

        UpdateTexts(_selectedPlanet);
    }

    private void UpdateState()
    {
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        if (currentPlanet == null || currentPlanet.Anonymous())
        {
            TryHide();
        }
        else
        {
            if (HiddenAlready()) Show();
            _selectedPlanet = currentPlanet;
        }
    }

    private void TryHide()
    {
        if (HiddenAlready()) return;
        StartHide();
    }

    private void Show()
    {
        _animator.SetBool(Visible, true);
    }

    private void StartHide()
    {
        _selectedPlanet = null;
        _animator.SetBool(Visible, false);
    }

    private void UpdateTexts(TinyPlanet planet)
    {
        var resources = planet.GetResources();

        var header = headerText;
        if (!header.gameObject.activeSelf) header.gameObject.SetActive(true);
        header.text = planet.planetName;

        var resourceToShow = new List<TinyPlanetResources.PlanetResourceType>();
        foreach (var resource in _resources)
        {
            var amount = Mathf.FloorToInt(resources.GetResource(resource));
            if (amount > 0 || _tutorialItems.Contains(resource))
            {
                resourceToShow.Add(resource);
            }
            else if (resource == TinyPlanetResources.PlanetResourceType.Food && resources.HasFarm())
            {
                resourceToShow.Add(resource);
            }
            else if (resource == TinyPlanetResources.PlanetResourceType.Energy && resources.HasPowerPlant())
            {
                resourceToShow.Add(resource);
            }
        }

        ShowResources(resources, resourceToShow);
    }

    private void ShowResources(TinyPlanetResources resources,
        IReadOnlyList<TinyPlanetResources.PlanetResourceType> resourcesToShow)
    {
        var text = "";
        for (var i = 0; i < resourcesToShow.Count; i++)
        {
            var resource = resourcesToShow[i];
            var amount = Mathf.FloorToInt(resources.GetResource(resource));

            text += $"{TinyPlanetResources.ResourceName(resource)}:{amount} ";
        }

        resourcesText.text = text;
    }

    private bool IsVisible()
    {
        return _animator.GetBool(Visible);
    }

    private bool HiddenAlready()
    {
        return !IsVisible();
    }
}