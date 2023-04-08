using System;
using UnityEngine;

public class StartingSequenceCameraController : MonoBehaviour
{
    private static StartingSequenceCameraController _instance;

    private bool _movingToShip;

    public StartingShip startingShip;
    private bool _enteringShip;

    public event Action FinishedOpening;
    public event Action FinishedEnteringShip;

    public static StartingSequenceCameraController Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        CameraController.Get().OnNavigationFinished += CameraNavigationFinished;
    }

    public void FocusOnStartingShip()
    {
        _movingToShip = true;

        CurrentPlanetController.Get().FocusOnShip(startingShip.GetConvoyBeacon());

        CameraController.Get().FocusOnTarget(startingShip.transform, false);
    }

    public void EnterShip()
    {
        _enteringShip = true;

        CameraController.Get().EnterInside(startingShip.transform, false);

        startingShip.StartHiding();
    }

    public void ForceFocusOnStartingShip()
    {
        CameraController.Get().ForceFocusOnTarget(startingShip.transform);
    }

    private void CameraNavigationFinished()
    { 
        if (_movingToShip)
        {
            _movingToShip = false;
            FinishedOpening?.Invoke();
        }
        else if (_enteringShip)
        {
            _enteringShip = false;
            FinishedEnteringShip?.Invoke();
        }
    }
}