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

    void Start()
    {
        _emptySlotController = GetComponentInChildren<EmptySlotController>();
        
        _emptyCargoSlotIcons = GetComponentInChildren<EmptyCargoSlotIcons>();
        _emptyCargoSlotIcons.UpdateIcons();
        _emptyCargoSlotIcons.ResourceSelected += SelectResource;

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

        SlotFilled?.Invoke();
    }

    public void Reset()
    {
        emptyCargoSlot.SetActive(true);
        filledCargoSlot.SetActive(false);

        _emptyCargoSlotIcons.UpdateIcons();
    }

    public void UpdateOnShow()
    {
        if (emptyCargoSlot.activeSelf)
        {
            GetComponentInChildren<EmptyCargoSlotIcons>().UpdateIcons();
        }
    }

    public bool IsFilled()
    {
        return filledCargoSlot.activeSelf;
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
}