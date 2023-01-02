using System.Collections.Generic;
using System.Linq;
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
            TinyPlanetResources.PlanetResourceType.Inhabitants,
            TinyPlanetResources.PlanetResourceType.Energy,
            TinyPlanetResources.PlanetResourceType.Food,
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.Metals,
            TinyPlanetResources.PlanetResourceType.Gadgets,
            TinyPlanetResources.PlanetResourceType.Housing,
        };

    // When there are a lack of resources - show these as 0, in this order, to guide the player on what to do.
    private readonly List<TinyPlanetResources.PlanetResourceType> _tutorialItems =
        new()
        {
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.Housing,
            TinyPlanetResources.PlanetResourceType.Inhabitants,
        };

    // When there are less resources of more than 0, then the amount of tutorial items, show items in this order to ensure tutorial items remain on top:
    private readonly List<TinyPlanetResources.PlanetResourceType> _tutorialOrder =
        new()
        {
            TinyPlanetResources.PlanetResourceType.Ore,
            TinyPlanetResources.PlanetResourceType.Housing,
            TinyPlanetResources.PlanetResourceType.Inhabitants,
            TinyPlanetResources.PlanetResourceType.Energy,
            TinyPlanetResources.PlanetResourceType.Food,
            TinyPlanetResources.PlanetResourceType.Metals,
            TinyPlanetResources.PlanetResourceType.Gadgets,
        };

    private TMP_Text[] _texts;

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

    public void Show(Vector2 position, TinyPlanet planet)
    {
        _selectedPlanet = planet;

        UpdateTexts(_selectedPlanet);
        UpdatePosition(position);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            _animator.SetBool(Visible, true);
        }
    }

    private void UpdateTexts(TinyPlanet planet)
    {
        // Disable all texts (except header) - then enable only those needed further down.
        for (var i = ResourceTextsStartingIndex; i < _texts.Length; i++)
        {
            _texts[i].gameObject.SetActive(false);
        }

        var resources = planet.GetResources();

        var header = _texts[0];
        if (!header.gameObject.activeSelf) header.gameObject.SetActive(true);
        header.text = planet.planetName;

        var resourceToShow = new List<TinyPlanetResources.PlanetResourceType>();

        foreach (var resource in _resources)
        {
            var amount = resources.GetResource(resource);
            if (amount > 0)
            {
                resourceToShow.Add(resource);
            }
        }

        if (resourceToShow.Count < _tutorialItems.Count)
        {
            foreach (var tutorialItem in _tutorialItems)
            {
                if (!resourceToShow.Contains(tutorialItem)) resourceToShow.Add(tutorialItem);
            }
        }

        if (resourceToShow.Count <= 3)
        {
            ShowResources(resources, resourceToShow.OrderBy(r => _tutorialOrder.IndexOf(r)).ToArray());
        }
        else
        {
            ShowResources(resources, resourceToShow.OrderBy(r => _resources.IndexOf(r)).ToArray());
        }
    }

    private void ShowResources(TinyPlanetResources resources, TinyPlanetResources.PlanetResourceType[] resourcesToShow)
    {
        for (var i = 0; i < resourcesToShow.Length; i++)
        {
            var resource = resourcesToShow[i];
            var amount = resources.GetResource(resource);

            _texts[ResourceTextsStartingIndex + i].gameObject.SetActive(true);
            _texts[ResourceTextsStartingIndex + i].text = $"{TinyPlanetResources.ResourceName(resource)}: {amount}";
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        _selectedPlanet = null;
        gameObject.SetActive(false);
    }

    public void StartHide()
    {
        _animator.SetBool(Visible, false);
    }

    public bool HiddenAlready()
    {
        return !gameObject.activeSelf;
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