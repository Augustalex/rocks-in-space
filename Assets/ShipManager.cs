using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public GameObject startingShipTemplate;

    public List<ControllableShip> _ships = new();

    private static ShipManager _instance;

    public static ShipManager Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    public ControllableShip CreateStartingShip()
    {
        var ship = Instantiate(startingShipTemplate);
        var controllable = ship.GetComponent<ControllableShip>();

        _ships.Add(controllable);

        return controllable;
    }
}