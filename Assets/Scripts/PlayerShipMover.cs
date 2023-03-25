using System;
using Game;
using UnityEngine;

public class PlayerShipMover
{
    private PlanetId _target;
    private float _startedMoving;
    private PlanetId _current;
    private float _cancelledMoveAt;
    private Vector3 _targetPosition;

    private const float Speed = 30f;

    public enum ShipState
    {
        ErrorStranded,
        ErrorReturning,
        Moving,
        Arrived,
        StartingPosition,
    }

    private ShipState _shipState = ShipState.StartingPosition;
    private float _lastKnownProgress;
    private Vector3 _lastKnownPosition;
    private Vector3 _startPosition;

    public event Action<ShipState> StateChanged;

    public void PutInStartingPosition(Vector3 startingPosition)
    {
        _current = null;

        _lastKnownPosition = startingPosition;
        _startPosition = startingPosition;

        ChangeState(ShipState.StartingPosition);
    }

    public void MoveToPlanet(TinyPlanet targetPlanet)
    {
        if (_shipState == ShipState.Moving)
        {
            ChangeDirection(targetPlanet);
        }
        else if (_shipState == ShipState.Arrived || _shipState == ShipState.StartingPosition)
        {
            _target = targetPlanet.PlanetId;
            _targetPosition = targetPlanet.GetCenter();
            _startedMoving = GameTime.Time();
            UpdateStartingPosition();

            ChangeState(ShipState.Moving);
        }
        else
        {
            Debug.LogError("Tried to move ship while either returning or stranded");
        }
    }

    private void ChangeDirection(TinyPlanet newTargetPlanet)
    {
        _lastKnownProgress = 0f;

        _current = null;
        _startPosition = _lastKnownPosition;

        _target = newTargetPlanet.PlanetId;
        _targetPosition = newTargetPlanet.GetCenter();

        _startedMoving = GameTime.Time();

        ChangeState(ShipState.Moving);
    }

    private void UpdateStartingPosition()
    {
        if (_current != null)
        {
            var current = PlanetsRegistry.Get().GetPlanet(_current);
            if (current)
            {
                _startPosition = current.GetCenter();
            }
            else
            {
                // Use previously stored position
            }
        }
        else
        {
            // Use previously stored position
        }
    }

    public void Progress()
    {
        var (current, target) = GetCurrentAndTargetOrReset();

        if (_shipState == ShipState.ErrorReturning)
        {
            // target is null, current COULD be null (if starting position was not a planet)

            var homePosition = current ? current.GetCenter() : _startPosition;

            var cancelledDuration = _cancelledMoveAt - _startedMoving;
            var cancelledAtProgress = (cancelledDuration * Speed) / Vector3.Distance(homePosition, _targetPosition);
            var turningPoint = Mathf.Clamp(cancelledAtProgress, 0f, 1f);

            var movingDuration = GameTime.Time() - _cancelledMoveAt;
            var progress = (movingDuration * Speed) / (Vector3.Distance(homePosition, _targetPosition) * turningPoint);
            var clampedProgress = Mathf.Clamp(progress, 0f, 1f);
            _lastKnownProgress = clampedProgress;
        }
        else if (_shipState == ShipState.ErrorStranded)
        {
        }
        else if (_shipState == ShipState.Moving)
        {
            // NEITHER current NOR target is null

            var movingDuration = GameTime.Time() - _startedMoving;
            var progress = (movingDuration * Speed) / Vector3.Distance(_startPosition, target.GetCenter());
            var clampedProgress = Mathf.Clamp(progress, 0f, 1f);
            _lastKnownProgress = clampedProgress;

            _lastKnownPosition = Vector3.Lerp(_startPosition, _targetPosition, _lastKnownProgress);

            if (_lastKnownProgress >= 1f)
            {
                Arrived(target);
            }
        }
        else if (_shipState == ShipState.Arrived)
        {
        }
    }

    public Vector3 ReadPosition()
    {
        return _lastKnownPosition;
    }

    public float ReadProgress()
    {
        return _lastKnownProgress;
    }

    public bool HasArrived()
    {
        return _shipState == ShipState.Arrived;
    }

    public ShipState GetState()
    {
        return _shipState;
    }

    private (TinyPlanet current, TinyPlanet target) GetCurrentAndTargetOrReset()
    {
        TinyPlanet current = null;

        if (_current != null)
        {
            current = PlanetsRegistry.Get().GetPlanet(_current);
            if (!current)
            {
                Debug.LogError("PlayerShip: Current planet no longer exists.");
                Stranded();

                return (null, null);
            }
        }
        else
        {
            // Starting from a position, not a planet
        }

        var target = PlanetsRegistry.Get().GetPlanet(_target);
        if (!target)
        {
            Debug.LogError("PlayerShip: Target planet no longer exists, aborting move.");
            CancelMove();

            return (null, null);
        }

        return (current, target);
    }

    public void Arrived(TinyPlanet targetPlanet)
    {
        _current = targetPlanet.PlanetId;
        _lastKnownProgress = 0f;
        _startPosition = targetPlanet.GetCenter();
        ChangeState(ShipState.Arrived);
    }

    private void Stranded()
    {
        _lastKnownPosition = Vector3.Lerp(_startPosition, _targetPosition, _lastKnownProgress);
        _target = null;
        _current = null;
        ChangeState(ShipState.ErrorStranded);
    }

    private void CancelMove()
    {
        _cancelledMoveAt = GameTime.Time();
        _target = null;
        ChangeState(ShipState.ErrorReturning);
    }

    private void ChangeState(ShipState newState)
    {
        _shipState = newState;
        StateChanged?.Invoke(newState);
    }

    public PlanetId CurrentPlanet()
    {
        return _current;
    }
}