using System;
using UnityEngine;

public class MapPopup : MonoBehaviour
{
    private Animator _animator;
    private TinyPlanet _selectedPlanet;

    public GameObject nameSectionRoot;
    public GameObject resourceSectionRoot;

    public enum MapPopupMode
    {
        Name,
        Resources,
        Connecting,
        ConnectionTarget
    }

    private MapPopupMode _mode = MapPopupMode.Name;
    private MapPopupNameSection _nameSection;
    private MapPopupResourcesSection _resourcesSection;
    private bool _routing;

    public event Action Hidden;

    public static GameObject GetNewInstance()
    {
        var template = PrefabTemplateLibrary.Get().mapPopup;
        var canvas = FindObjectOfType<Canvas>();
        var newInstance = Instantiate(template, canvas.transform, true);
        return newInstance;
    }

    private void Awake()
    {
        _nameSection = nameSectionRoot.GetComponentInChildren<MapPopupNameSection>();
        _resourcesSection = resourceSectionRoot.GetComponentInChildren<MapPopupResourcesSection>();

        nameSectionRoot.SetActive(false);
        resourceSectionRoot.SetActive(false);
    }

    public void Show(Vector2 position, TinyPlanet planet)
    {
        return;
        _selectedPlanet = planet;
        UpdateMode();

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        UpdatePosition(position);
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (!_selectedPlanet) return;

        if (_mode == MapPopupMode.Name)
        {
            if (!nameSectionRoot.activeSelf) nameSectionRoot.SetActive(true);
            if (resourceSectionRoot.activeSelf) resourceSectionRoot.SetActive(false);

            _nameSection.UpdateTexts(_selectedPlanet);
        }
        else if (_mode == MapPopupMode.Resources)
        {
            if (nameSectionRoot.activeSelf) nameSectionRoot.SetActive(false);
            if (!resourceSectionRoot.activeSelf) resourceSectionRoot.SetActive(true);

            _resourcesSection.UpdateTexts(_selectedPlanet);
        }
    }

    public void UpdatePosition(Vector2 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        _selectedPlanet = null;

        Hidden?.Invoke();

        Destroy(this.gameObject);
    }

    public bool HiddenAlready()
    {
        return !gameObject.activeSelf;
    }

    public bool ShownFor(TinyPlanet planet)
    {
        return _selectedPlanet == planet;
    }

    public void UpdateMode()
    {
        if (_routing)
        {
            _mode = MapPopupMode.Connecting;
        }
        if (_selectedPlanet == null) return;
        
        
        var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
        _mode = currentPlanet == _selectedPlanet ? MapPopupMode.Resources : MapPopupMode.Name;
    }

    public void StartRouting()
    {
        _routing = true;
    }
}