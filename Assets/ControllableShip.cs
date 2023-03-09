using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ControllableShip : MonoBehaviour
{
    private TinyPlanet _target;

    private ShipStatus _status = ShipStatus.Waiting;
    private double _updateCenterAt;
    private Vector3 _center;

    public enum ShipStatus
    {
        Moving,
        Orbiting,
        Waiting
    }

    void Update()
    {
        if (_status == ShipStatus.Orbiting)
        {
            if (Time.time > _updateCenterAt)
            {
                UpdateCenter();
            }
        }
    }

    public void MoveToPlanetInstantly(PlanetId planetId)
    {
        var planet = PlanetsRegistry.Get().GetPlanet(planetId);

        _target = planet;
        _status = ShipStatus.Orbiting;
        
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        UpdateCenter();

        var distance = 5f;
        do
        {
            distance += 2f;
            transform.position = _center + Vector3.forward * distance;
        } while (Physics.OverlapSphere(transform.position, 4f).Any(h => h.GetComponent<Block>()));
    }

    private void UpdateCenter()
    {
        _updateCenterAt = Time.time + 30;
        _center = _target.GetCenter();
    }
}