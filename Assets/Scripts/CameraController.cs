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
    private float _moveLength;
    private bool _moving;
    private Vector3 _lastPosition;
    private bool _following;

    public bool cinematicOpening = true;
    private static CameraController _instance;
    private bool _zoomedOut;
    public event Action<bool> OnToggleZoom;

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
            SelectInteractor.Get()
                .ForceSetLastConnectedPlanet(startingPlanet); // TODO: Fix circular dependency on SelectInteractor
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
                if(_displayController.inputMode == DisplayController.InputMode.Cinematic) _displayController.ExitCinematicMode();
            }
        }
        else if(_displayController.inputMode == DisplayController.InputMode.Static)
        {
            if (_focus) _backupFocus = _focus.position;

            if (!_following && _focus)
            {
                _following = true;
                _lastPosition = _focus.position;
            }
            else if (_following && _focus)
            {
                var focusPosition = _focus.position;
                _camera.transform.position += focusPosition - _lastPosition;
                _lastPosition = focusPosition;
            }

            if (Input.GetKey(KeyCode.A))
            {
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, 45f * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, -45f * Time.deltaTime);
            }

            var maxTilt = 75f;
            var eulerAnglesX = _camera.transform.rotation.eulerAngles.x;
            var adjustedAngles = eulerAnglesX > 180f ? (eulerAnglesX - 360f) : eulerAnglesX;
            if (Input.GetKey(KeyCode.Q) && adjustedAngles > -maxTilt)
            {
                _camera.transform.RotateAround(FocusPoint(), transform.right, -45f * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.E) && adjustedAngles < maxTilt)
            {
                _camera.transform.RotateAround(FocusPoint(), transform.right, 45f * Time.deltaTime);
            }

            if (!_zoomedOut)
            {
                var maxInnerZoom = 12f;
                var maxOuterZoom = 42f;
                var cameraTransform = _camera.transform;
                var distance = Vector3.Distance(FocusPoint(), cameraTransform.position);
                
                if (Input.GetKey(KeyCode.S) && distance < maxOuterZoom)
                {
                    cameraTransform.position += cameraTransform.forward * (-10f * Time.deltaTime);
                }
                else if (Input.GetKey(KeyCode.W) && distance > maxInnerZoom)
                {
                    cameraTransform.position += cameraTransform.forward * (10f * Time.deltaTime);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
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
            InteractorController.Get().LockToDefaultInteractor();
            var (targetPosition, targetRotation) = CameraPlanetZoomedOutPosition();
            var cameraTransform = _camera.transform;
            cameraTransform.position = targetPosition;
            cameraTransform.rotation = targetRotation;
        }
        else
        {
            InteractorController.Get().UnlockFromDefaultInteractor();
            var (targetPosition, targetRotation) = CameraPlanetZoomedInPosition();
            var cameraTransform = _camera.transform;
            cameraTransform.position = targetPosition;
            cameraTransform.rotation = targetRotation;
        }
        
        OnToggleZoom?.Invoke(_zoomedOut);
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

        var cameraTransform = _camera.transform;
        _startPosition = cameraTransform.position;
        _startRotation = cameraTransform.rotation;
        (_targetPosition, _targetRotation) = CameraPlanetFocusPosition(previousFocusPoint, _focus.position);

        var distance = (_targetPosition - _startPosition).magnitude;
        _moveLength = distance < 10f ? .5f : distance < 10f ? .75f : distance < 20f ? 1f : MaxMoveLength;
        _moveTime = _moveLength;
    }

    private Vector3 FocusPoint()
    {
        return _focus ? _focus.position : _backupFocus;
    }

    private Tuple<Vector3, Quaternion> CameraPlanetFocusPosition(Vector3 previousFocusPoint, Vector3 newFocusPoint)
    {
        if (_zoomedOut)
        {
            var toMove = newFocusPoint - previousFocusPoint;
            var cameraTransform = _camera.transform;
            return new Tuple<Vector3, Quaternion>(cameraTransform.position + toMove, cameraTransform.rotation);
        }
        else
        {
            var newPosition = newFocusPoint + Vector3.forward * 20f;
            var direction = (newFocusPoint - newPosition).normalized;
            return new Tuple<Vector3, Quaternion>(newPosition,
                Quaternion.LookRotation(direction, Vector3.up));
        }
    }

    private Tuple<Vector3, Quaternion> CameraPlanetZoomedOutPosition()
    {
        var cameraTransform = _camera.transform;
        var cameraPosition = cameraTransform.position;
        
        var distanceFromCenter = Vector3.Distance(FocusPoint(), cameraPosition);
        var distanceToMove = ZoomedOutDistance - distanceFromCenter;
        var targetPosition = cameraPosition + cameraTransform.forward * -distanceToMove;

        return new Tuple<Vector3, Quaternion>(targetPosition, cameraTransform.rotation);
    }

    private Tuple<Vector3, Quaternion> CameraPlanetZoomedInPosition()
    {
        var cameraTransform = _camera.transform;
        var cameraPosition = cameraTransform.position;

        var distanceFromCenter = Vector3.Distance(FocusPoint(), cameraPosition);
        var distanceToMove = distanceFromCenter - 20f;
        var targetPosition = cameraPosition + cameraTransform.forward * distanceToMove;

        return new Tuple<Vector3, Quaternion>(targetPosition, cameraTransform.rotation);
    }
}