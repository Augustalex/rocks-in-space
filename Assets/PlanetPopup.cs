using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetPopup : MonoBehaviour
{
    private static PlanetPopup _instance;
    private Animator _animator;
    private static readonly int Visible = Animator.StringToHash("Visible");
    private TinyPlanet _selectedPlanet;

    private const int
        ResourceTextsStartingIndex = 1; // The 0th text element is for the Header. From 1 and up, it's the resources.

    // If a planet has all these resources - then this is the optimal order to display them in.
    // It is also a mapping between the text elements in the popup - starting from the index below the header.
    private readonly List<TinyPlanetResources.PlanetResourceType> _resources =
        new()
        {
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.IronPlates,
            TinyPlanetResources.PlanetResourceType.Gadgets,
            TinyPlanetResources.PlanetResourceType.Energy,
            TinyPlanetResources.PlanetResourceType.Food,
            TinyPlanetResources.PlanetResourceType.Housing,
            TinyPlanetResources.PlanetResourceType.Inhabitants,
        };

    // When there are a lack of resources - show these as 0, in this order, to guide the player on what to do.
    private readonly List<TinyPlanetResources.PlanetResourceType> _tutorialItems =
        new()
        {
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.IronPlates,
            TinyPlanetResources.PlanetResourceType.Gadgets,
            TinyPlanetResources.PlanetResourceType.Inhabitants,
        };

    private TMP_Text[] _texts;

    private const PopupManager.PopupImportance PopupImportance = PopupManager.PopupImportance.Low;
    private int _popupId;

    public static PlanetPopup Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
        _animator = GetComponent<Animator>();

        _texts = GetComponentsInChildren<TMP_Text>();
        for (var i = 1; i < _texts.Length; i++)
        {
            _texts[i].gameObject.SetActive(false);
        }

        Hide();
    }

    private void Start()
    {
        CameraController.Get().OnToggleZoom += ZoomToggled;

        var popupManager = PopupManager.Get();
        _popupId = popupManager.Register();
        popupManager.PopupShown += AnotherPopupShown;
        popupManager.RequestedCancel += Hide;
    }

    private void AnotherPopupShown(PopupManager.PopupImportance popupImportance, int popupId)
    {
        if (popupId == _popupId) return;
        if (popupImportance >= PopupImportance) Hide();
    }

    public void Show(Vector2 position, TinyPlanet planet)
    {
        _selectedPlanet = planet;

        UpdateInformation(position, planet);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            _animator.SetBool(Visible, true);
        }

        PopupManager.Get().NotifyShown(PopupImportance, _popupId);
    }

    public void StartHide()
    {
        // Triggers FadeOut animation that eventually calls Hide().
        _animator.SetBool(Visible, false);
    }

    public void Hide()
    {
        // Hides right away - is called by the FadeOut animation as well when triggered from StartHide().

        _selectedPlanet = null;
        gameObject.SetActive(false);
    }

    private void ZoomToggled(bool zoomedOut)
    {
        Hide();
    }

    public void UpdateInformation(Vector2 position, TinyPlanet connectedPlanet)
    {
        UpdateTexts(connectedPlanet);
        UpdatePosition(position);
    }

    private void UpdateTexts(TinyPlanet planet)
    {
        // Disable all texts (except header) - then enable only those needed further down.
        // for (var i = ResourceTextsStartingIndex; i < _texts.Length; i++)
        // {
        //     _texts[i].gameObject.SetActive(false);
        // }
        //
        // var resources = planet.GetResources();

        var header = _texts[0];
        if (!header.gameObject.activeSelf) header.gameObject.SetActive(true);
        header.text = planet.planetName;

        // var resourceToShow = new List<TinyPlanetResources.PlanetResourceType>();
        // foreach (var resource in _resources)
        // {
        //     var amount = Mathf.FloorToInt(resources.GetResource(resource));
        //     if (amount > 0 || _tutorialItems.Contains(resource))
        //     {
        //         resourceToShow.Add(resource);
        //     }
        //     else if (resource == TinyPlanetResources.PlanetResourceType.Food && resources.HasFarm())
        //     {
        //         resourceToShow.Add(resource);
        //     }
        //     else if (resource == TinyPlanetResources.PlanetResourceType.Energy && resources.HasPowerPlant())
        //     {
        //         resourceToShow.Add(resource);
        //     }
        // }
        //
        // ShowResources(resources, resourceToShow);
    }

    private void ShowResources(TinyPlanetResources resources,
        IReadOnlyList<TinyPlanetResources.PlanetResourceType> resourcesToShow)
    {
        for (var i = 0; i < resourcesToShow.Count; i++)
        {
            var resource = resourcesToShow[i];
            var amount = Mathf.FloorToInt(resources.GetResource(resource));

            _texts[ResourceTextsStartingIndex + i].gameObject.SetActive(true);
            _texts[ResourceTextsStartingIndex + i].text = $"{TinyPlanetResources.ResourceName(resource)}: {amount}";
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }

    public bool HiddenAlready()
    {
        return !IsVisible();
    }

    public bool ShownFor(TinyPlanet planet)
    {
        return _selectedPlanet == planet;
    }

    public bool StartedHiding()
    {
        return !_animator.GetBool(Visible) && !HiddenAlready();
    }
}