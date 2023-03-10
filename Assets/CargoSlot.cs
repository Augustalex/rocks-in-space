using System;
using UnityEngine;

public class CargoSlot : MonoBehaviour
{
    public GameObject emptyCargoSlot;
    public GameObject filledCargoSlot;

    public event Action SlotFilled;

    private EmptyCargoSlotIcons _emptyCargoSlotIcons;
    private FilledCargoSlotController _filledSlotController;
    private EmptySlotController _emptySlotController;

    private bool _filled = false;
    private TinyPlanetResources.PlanetResourceType _selectedResource;

    void Start()
    {
        _emptyCargoSlotIcons = GetComponentInChildren<EmptyCargoSlotIcons>();
        _emptyCargoSlotIcons.ResourceSelected += SelectResource;

        _emptySlotController = GetComponentInChildren<EmptySlotController>();
        _emptySlotController.UpdateSlot();

        _filledSlotController = GetComponentInChildren<FilledCargoSlotController>();
        _filledSlotController.UnloadedAll += Reset;

        emptyCargoSlot.SetActive(true);
        filledCargoSlot.SetActive(false);
    }

    public void LoadAmount(int amount)
    {
        _filledSlotController.Conjure(amount);
    }

    public void SelectResource(TinyPlanetResources.PlanetResourceType resource)
    {
        emptyCargoSlot.SetActive(false);
        filledCargoSlot.SetActive(true);

        _filledSlotController.SetResource(resource);

        _selectedResource = resource;
        _filled = true;

        SlotFilled?.Invoke();
    }

    public void Reset()
    {
        emptyCargoSlot.SetActive(true);
        filledCargoSlot.SetActive(false);

        _emptySlotController.UpdateSlot();

        _filled = false;
    }

    public void UpdateOnShow()
    {
        if (emptyCargoSlot.activeSelf)
        {
            _emptySlotController.UpdateSlot();
        }
    }

    public bool IsFilled()
    {
        return _filled;
    }

    public void Show()
    {
        _emptySlotController.Available();
        UpdateOnShow();
    }

    public void Hide()
    {
        _emptySlotController.Unavailable();
    }

    public bool HasResource(TinyPlanetResources.
        PlanetResourceType resourceType)
    {
        return _filled && _selectedResource == resourceType;
    }
}