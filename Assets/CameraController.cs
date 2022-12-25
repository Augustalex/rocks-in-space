using System;
using System.Collections;
using Interactors;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float MaxMoveLength = 2f;
    private const float ZoomedOutDistance = 250f;
    
    private Transform _focus;
    private Vector3 _backupFocus = Vector3.zero;
    private Camera _camera;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Vector3 _startPosition;
    private Quaternion _startRotation;
    private float _moveTime;
    private DisplayController _displayController;
    private float _moveLength = 0f;
    private bool _moving;
    private Vector3 _lastPosition;
    private bool _following;

    public bool cinematicOpening = true;
    private static CameraController _instance;
    private bool _zoomedOut;

    void Awake()
    {
        _instance = this;
    }

    public static CameraController Get()
    {
        return _instance;
    }

    void Start()
    {
        _camera = GetComponent<Camera>();
        _focus = null;
        _displayController = DisplayController.Get();

        StartCoroutine(DoSoon());

        IEnumerator DoSoon()
        {
            yield return new WaitForSeconds(.5f);

            var startingPlanet = FindObjectOfType<Block>().GetConnectedPlanet();
            
            //Game start
            CurrentPlanetController.Get().ChangePlanet(startingPlanet);
            SelectInteractor.Get().ForceSetLastConnectedPlanet(startingPlanet); // TODO: Fix circular dependency on SelectInteractor
            FocusOnPlanetSlowly(startingPlanet);
        }
    }

    public bool AvailableToUpdate()
    {
        return _displayController.inputMode != DisplayController.InputMode.Renaming;
    }
    
    void Update()
    {
        if (!AvailableToUpdate()) return;
        
        if (_moving)
        {
            if (_moveTime > 0)
            {
                var progress = (_moveLength - _moveTime) / _moveLength;

                transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, progress);
                // transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, progress);
                transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);

                _moveTime -= Time.deltaTime;
            }
            else
            {
                _moving = false;
                _displayController.ExitCinematicMode();
            }
        }
        else
        {
            if (_focus) _backupFocus = _focus.position;

            if (!_following && _focus)
            {
                _following = true;
                _lastPosition = _focus.position;
            }
            else if (_following && _focus)
            {
                _camera.transform.position += _focus.position - _lastPosition;
                _lastPosition = _focus.position;
            }

            // _camera.transform.LookAt(FocusPoint(), Vector3.up);

            if (Input.GetKey(KeyCode.A))
            {
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, 45f * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, -45f * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _camera.transform.RotateAround(FocusPoint(), transform.right, -45f * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                _camera.transform.RotateAround(FocusPoint(), -transform.right, -45f * Time.deltaTime);
            }

            if (!_zoomedOut)
            {
                if (Input.GetKey(KeyCode.S))
                {
                    _camera.transform.position += _camera.transform.forward * -10f * Time.deltaTime;
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    _camera.transform.position += _camera.transform.forward * 10f * Time.deltaTime;
                }
            }

            if (_displayController.inputMode == DisplayController.InputMode.Static && Input.GetKeyDown(KeyCode.Space))
            {
                ToggleZoomMode();
            }
        }
    }

    private void ToggleZoomMode()
    {
        _zoomedOut = !_zoomedOut;
        if (_zoomedOut)
        {
            var (targetPosition, targetRotation) = CameraPlanetZoomedOutPosition();
            _camera.transform.position = targetPosition;
            _camera.transform.rotation = targetRotation;
        }
        else
        {
            var (targetPosition, targetRotation) = CameraPlanetZoomedInPosition();
            _camera.transform.position = targetPosition;
            _camera.transform.rotation = targetRotation;
        }
    }

    private void FocusOnPlanetSlowly(TinyPlanet planet)
    {
        FocusOnPlanet(planet);
        _displayController.SetToCinematicMode();
        _moveLength = cinematicOpening ? 10 : .1f; 
        _moveTime = _moveLength;
    }

    public void FocusOnPlanet(TinyPlanet planet)
    {
        _moving = true;
        _following = false;

        _displayController.SetPlanetInFocus(planet);

        var center = TinyPlanetCenterPointHelper.GetMostCentralBlock(planet.network);
        var previousFocusPoint = _focus ? _focus.position : Vector3.zero;
        _focus = center.transform;
        
        _startPosition = _camera.transform.position;
        _startRotation = _camera.transform.rotation;

        (_targetPosition, _targetRotation) = CameraPlanetFocusPosition(previousFocusPoint, _focus.position);

        var distance = (_targetPosition - _startPosition).magnitude;
        _moveLength = distance < 10f ? .5f : distance < 10f ? .75f : distance < 20f ? 1f : MaxMoveLength;
        _moveTime = _moveLength;

        _camera.transform.position = _startPosition;
        _camera.transform.rotation = _startRotation;
    }

    private Vector3 FocusPoint()
    {
        return _focus ? _focus.position : _backupFocus;
    }

    private Tuple<Vector3, Quaternion> CameraPlanetFocusPosition(Vector3 previousFocusPoint, Vector3 newFocusPoint)
    {
        if (_zoomedOut)
        {
            var startPosition = _camera.transform.position;
            var startRotation = _camera.transform.rotation;

            var toMove = newFocusPoint - previousFocusPoint;
            _camera.transform.position += toMove;
            var targetPosition = _camera.transform.position;
            var targetRotation = _camera.transform.rotation;

            _camera.transform.position = startPosition;
            _camera.transform.rotation = startRotation;

            return new Tuple<Vector3, Quaternion>(targetPosition, targetRotation);
        }
        else
        {
            var startPosition = _camera.transform.position;
            var startRotation = _camera.transform.rotation;

            var focusPoint = newFocusPoint;

            _camera.transform.position = Vector3.zero;
            _camera.transform.rotation = Quaternion.identity;
            _camera.transform.LookAt(focusPoint, Vector3.up);
            _camera.transform.position = focusPoint;
            _camera.transform.position += _camera.transform.forward * -20f;

            var targetPosition = _camera.transform.position;
            var targetRotation = _camera.transform.rotation;

            _camera.transform.position = startPosition;
            _camera.transform.rotation = startRotation;

            return new Tuple<Vector3, Quaternion>(targetPosition, targetRotation);
        }
    }

    private Tuple<Vector3, Quaternion> CameraPlanetZoomedOutPosition()
    {
        var startPosition = _camera.transform.position;
        var startRotation = _camera.transform.rotation;

        var distanceFromCenter = Vector3.Distance(FocusPoint(), startPosition);
        var distanceToMove = ZoomedOutDistance - distanceFromCenter;
        _camera.transform.position += _camera.transform.forward * -distanceToMove;
        var targetPosition = _camera.transform.position;

        _camera.transform.position = startPosition;
        _camera.transform.rotation = startRotation;

        return new Tuple<Vector3, Quaternion>(targetPosition, startRotation);
    }

    private Tuple<Vector3, Quaternion> CameraPlanetZoomedInPosition()
    {
        var startPosition = _camera.transform.position;
        var startRotation = _camera.transform.rotation;

        var distanceFromCenter = Vector3.Distance(FocusPoint(), startPosition);
        var distanceToMove = distanceFromCenter - 20f;
        _camera.transform.position += _camera.transform.forward * distanceToMove;
        var targetPosition = _camera.transform.position;

        _camera.transform.position = startPosition;
        _camera.transform.rotation = startRotation;

        return new Tuple<Vector3, Quaternion>(targetPosition, startRotation);
    }
}