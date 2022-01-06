using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpacerNavigator : MonoBehaviour
{
    private GameObject _holdingMineral;
    private GameObject _targetMineral;
    private Rigidbody _rigidbody;
    private bool _spinning;
    private PlanetRelative _planetRelative;
    private float _startedHolding;
    private const float AdjustSpeed = 1.2f;
    private const float NormalSpeed = 100f;
    private const float OrbitThreshold = 10f;

    public event Action CurrentTargetDone;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _planetRelative = GetComponent<PlanetRelative>();
    }

    void Update()
    {
        if (_spinning)
        {
            transform.RotateAround(transform.position, Vector3.forward, 180f * Time.deltaTime);
        }
        else
        {
            if (TooFarAwayOutOfOrbit())
            {
                AdjustBackIntoOrbit();
            }

            if (_targetMineral)
            {
                if (CloseEnoughToTarget())
                {
                    PickupMineral();
                }
                else if (ReadyToDescendFromOrbit())
                {
                    MoveTowardsMineral();
                }
                else if (InOrbit())
                {
                    MoveTowardsMineralInOrbit();
                }
                else
                {
                    MoveTowardsOrbit();
                }
            }
            else if (_holdingMineral)
            {
                if (Time.time - _startedHolding > 3)
                {
                    ConsumeMineral();
                }
            }
        }
    }

    private void ConsumeMineral()
    {
        var planetResources = _planetRelative.tinyPlanet.GetResources();
        planetResources.SetOre(planetResources.GetOre() + 1000);
        Destroy(_holdingMineral);
        _holdingMineral = null;

        OnCurrentTargetDone();
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
        Vector3 center = GetCenter();
        var position = transform.position;
        var vectorAwayFromCenter = (position - center);
        var direction = vectorAwayFromCenter.normalized;

        _rigidbody.AddForce(direction * NormalSpeed * Time.deltaTime, ForceMode.Acceleration);
    }

    private Vector3 GetCenter()
    {
        return _planetRelative.tinyPlanet.GetCenter();
    }

    private void MoveTowardsMineralInOrbit()
    {
        var targetDirectVector = (_targetMineral.transform.position - transform.position);
        var targetDirectDirection = targetDirectVector.normalized;
        _rigidbody.AddForce(targetDirectDirection * NormalSpeed * Time.deltaTime, ForceMode.Acceleration);
    }

    private bool InOrbit()
    {
        var center = GetCenter();
        var distanceFromCenter = Vector3.Distance(transform.position, center);

        return distanceFromCenter > OrbitThreshold;
    }

    private bool ReadyToDescendFromOrbit()
    {
        return false;
    }

    private bool CloseEnoughToTarget()
    {
        var distance = Vector3.Distance(_targetMineral.transform.position, transform.position);
        return distance < 1.5f;
    }

    private void PickupMineral()
    {
        _targetMineral.transform.SetParent(transform);
        _targetMineral.GetComponent<Rigidbody>().isKinematic = true;

        _holdingMineral = _targetMineral;
        _targetMineral = null;

        _startedHolding = Time.time;
    }

    private void MoveTowardsMineral()
    {
        var direction = (_targetMineral.transform.position - transform.position).normalized;
        _rigidbody.AddForce(direction * NormalSpeed * Time.deltaTime, ForceMode.Acceleration);
    }

    public bool IsFree()
    {
        return !_holdingMineral && !_targetMineral;
    }

    public void TargetMineral(GameObject floatingMineral)
    {
        _targetMineral = floatingMineral;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_targetMineral || _holdingMineral) return;
        return;
        _spinning = true;
        GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * .9f, ForceMode.Impulse);
    }

    protected virtual void OnCurrentTargetDone()
    {
        CurrentTargetDone?.Invoke();
    }
}