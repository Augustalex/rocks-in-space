using System.Linq;
using UnityEngine;

public class ShipInventoryCargoSlots : MonoBehaviour
{
    private CargoSlot[] _slots;

    private void Start()
    {
        var slots = GetComponentsInChildren<CargoSlot>();
        _slots = slots;

        foreach (var slot in _slots)
        {
            slot.SlotFilled += UpdateOnShow;
        }
    }

    public void UpdateOnShow()
    {
        var showNext = true;
        for (int i = 0; i < _slots.Length; i++)
        {
            var slot = _slots[i];
            if (showNext)
            {
                slot.Show();

                if (!slot.IsFilled())
                {
                    showNext = false;
                }
            }
            else
            {
                slot.Hide();
            }
        }
    }

    public CargoSlot GetFirstAvailableCargoSlot()
    {
        if (_slots.All(s => s.IsFilled())) return null;
        return _slots.First(s => !s.IsFilled());
    }

    public bool AnySlotHasResource(TinyPlanetResources.PlanetResourceType resourceType)
    {
        return _slots.Any(s => s.HasResource(resourceType));
    }
}