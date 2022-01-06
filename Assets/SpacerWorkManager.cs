using System;
using UnityEngine;

[RequireComponent(typeof(PlanetRelative))]
[RequireComponent(typeof(SpacerNavigator))]
public class SpacerWorkManager : MonoBehaviour
{
    public TinyPlanet tinyPlanet;

    public enum Actions
    {
        Mining,
        Idle
    }

    private Actions _currentAction = Actions.Idle;
    private SpacerNavigator _navigator;

    private void Awake()
    {
        tinyPlanet = GetComponent<PlanetRelative>().tinyPlanet;
        _navigator = GetComponent<SpacerNavigator>();
    }

    public bool CanMine()
    {
        return _currentAction == Actions.Idle;
    }

    public void Mine(GameObject mineral)
    {
        if (mineral == null)
        {
            Debug.Log("MINERAL IS NULL");
            return;
        }
        if (_navigator == null)
        {
            Debug.Log("NAVIGATOR IS NULL");
            return;
        }
        
        _currentAction = Actions.Mining;
        _navigator.TargetMineral(mineral);
        _navigator.CurrentTargetDone += OnDoneMining;

        void OnDoneMining()
        {
            _navigator.CurrentTargetDone -= OnDoneMining;
            
            _currentAction = Actions.Idle;
        }
    }
}