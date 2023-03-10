using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public ShipInventoryCargoSlots cargoSlots;
    
    private static PlayerInventory _instance;

    public static PlayerInventory Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
    
    public void AddResource(TinyPlanetResources.PlanetResourceType resourceType, int amount)
    {
        var slot = cargoSlots.GetFirstAvailableCargoSlot();
        if (!slot) return;

        slot.SelectResource(resourceType);
        slot.LoadAmount(amount);
    }
}