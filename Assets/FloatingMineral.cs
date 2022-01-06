using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlanetRelative))]
public class FloatingMineral : MonoBehaviour
{
    private Vector3 _lastActionPosition;
    private Rigidbody _rigidbody;
    private float _lastActionTime;
    private bool _spinning;
    private PlanetRelative _planetRelative;
    private Vector3 _targetDirection;
    private const float AdjustSpeed = 1.2f;
    private const float NormalSpeed = 15f;
    private const float OrbitThreshold = 10f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _planetRelative = GetComponent<PlanetRelative>();
    }

    void Start()
    {
        _targetDirection = Camera.main.transform.position - transform.position;
    }

    void Update()
    {
        if (TooFarAwayOutOfOrbit())
        {
            AdjustBackIntoOrbit();
        }
        
        if (!InOrbit())
        {
            MoveTowardsOrbit();
        }
    }

    private void AdjustBackIntoOrbit()
    {
        Vector3 center = GetCenter();
        var position = transform.position;
        var direction = -(position - center).normalized;
        _rigidbody.AddForce(direction * AdjustSpeed * Time.deltaTime, ForceMode.Acceleration);
    }

    private bool TooFarAwayOutOfOrbit()
    {
        var center = GetCenter();
        var distanceFromCenter = Vector3.Distance(transform.position, center);

        return distanceFromCenter - OrbitThreshold > 2;
    }

    private void MoveTowardsOrbit()
    {
        _rigidbody.AddForce(_targetDirection * NormalSpeed * Time.deltaTime, ForceMode.Acceleration);
    }

    private Vector3 GetCenter()
    {
        return _planetRelative.tinyPlanet.GetCenter();
    }

    private bool InOrbit()    
    {
        var center = GetCenter();
        var distanceFromCenter = Vector3.Distance(transform.position, center);

        return distanceFromCenter > OrbitThreshold;
    }
}
