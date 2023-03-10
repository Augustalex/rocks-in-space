using System;
using System.Collections;
using System.Linq;
using GameNotifications;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    private const float ZoomedOutDistance = 600f;
    private const float MinZoomedOutDistance = 400f;
    private const float MaxZoomedOutDistance = 1200f;
    private const float ZoomedOutSpeed = 320f;

    private const float ZoomedInDistance = 28f;
    private const float MinZoomedInDistance = 8f;
    private const float MaxZoomedInDistance = 90f;
    private const float ZoomedInSpeed = 10f;

    private const float MinShipZoomedInDistance = 75f;
    private const float ShipZoomedInDistance = 80f;

    private const float MouseLateralSpeed = 180f;
    private const float MouseMedialSpeed = 180f;

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
    public StartingShip startingShip;

    private static CameraController _instance;
    private static bool _hasInstance;
    private bool _zoomedOut;
    private MapPopupTarget _currentTarget;
    private bool _hitLimit;
    private bool _locked;
    private bool _enteringShip;
    private bool _movingToShip;

    public event Action<bool> OnToggleZoom;
    public event Action OnNavigationStarted;
    public event Action OnNavigationFinished;

    void Awake()
    {
        _instance = this;
        _hasInstance = true;
    }

    public static bool HasInstance()
    {
        return _hasInstance;
    }

    public static CameraController Get()
    {
        return !_hasInstance ? null : _instance;
    }

    public static Camera GetCamera()
    {
        return _instance._camera;
    }

    public void LockControls()
    {
        _locked = true;
    }

    public void UnlockControls()
    {
        _locked = false;
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

            var startingPlanet = FindStartingPlanet();

            //Game start on planet
            // CurrentPlanetController.Get().ChangePlanet(startingPlanet);
            // FocusOnPlanetSlowly(startingPlanet);

            //Game start on ship
            FocusOnStartingShip();
            //What happens:
            // 1. Zoom to ship
            // 2. Enter ship
            // 3. Collect all gifts
            // 4. Go to map view -> End sequence and change to Display mode "Static"
        }
    }

    private TinyPlanet FindStartingPlanet()
    {
        var planets = FindObjectsOfType<TinyPlanet>();
        var normalPlanet = planets.First(p => !p.IsIcePlanet());

        return normalPlanet;
    }

    public bool AvailableToUpdate()
    {
        return _displayController.inputMode != DisplayController.InputMode.Renaming &&
               _displayController.inputMode != DisplayController.InputMode.Cinematic &&
               _displayController.inputMode != DisplayController.InputMode.InventoryOnly;
    }

    void Update()
    {
        if (_moving)
        {
            if (_moveTime < _moveLength)
            {
                ProgressMove();
            }
            else
            {
                ClampAndFinishMove();

                if (_movingToShip)
                {
                    _movingToShip = false;
                }
                else if (_enteringShip)
                {
                    _enteringShip = false;

                    FinishedEnteringShip();
                }
                else if (_displayController.inputMode == DisplayController.InputMode.Cinematic)
                {
                    _displayController.ExitCinematicMode();
                }
            }

            return;
        }

        if (!AvailableToUpdate()) return;

        if (_displayController.inputMode == DisplayController.InputMode.Static)
        {
            HandleStaticMovement();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_moving)
            {
                AbortMoveAndToggleZoom();
            }
            else
            {
                ToggleZoomMode();
            }
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (CurrentPlanetController.Get().IsShipSelected())
            {
                // FocusOnShip(CurrentPlanetController.Get().CurrentShip());
            }
            else
            {
                var currentPlanet = CurrentPlanetController.Get().CurrentPlanet();
                if (currentPlanet)
                {
                    FocusOnPlanet(currentPlanet);
                }
            }
        }
    }

    private void HandleStaticMovement()
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

        var xDelta = Input.GetAxis("Mouse X");
        var yDelta = -Input.GetAxis("Mouse Y");

        var rightClickOn = Input.GetMouseButton(1);
        var goingLeft = rightClickOn && xDelta < 0.01f || Input.GetKey(KeyCode.A);
        var goingRight = rightClickOn && xDelta > 0.01f || Input.GetKey(KeyCode.D);
        var goingUp = rightClickOn && yDelta > 0.01f || Input.GetKey(KeyCode.Q);
        var goingDown = rightClickOn && yDelta < 0.01f || Input.GetKey(KeyCode.E);

        if (rightClickOn)
        {
            if (goingLeft || goingRight)
            {
                var lateralMovement = MouseLateralSpeed * xDelta;
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, lateralMovement * Time.deltaTime);
            }
        }
        else
        {
            if (goingLeft)
            {
                var leftMovementSpeed = 45f;
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, leftMovementSpeed * Time.deltaTime);
            }
            else if (goingRight)
            {
                var rightMovementSpeed = -45f;
                _camera.transform.RotateAround(FocusPoint(), Vector3.up, rightMovementSpeed * Time.deltaTime);
            }
        }

        var cameraTransform = _camera.transform;
        var previousRotation = cameraTransform.rotation;
        var previousPosition = cameraTransform.position;

        if (rightClickOn)
        {
            if (goingUp || goingDown)
            {
                var medialMovement = MouseMedialSpeed * yDelta;
                _camera.transform.RotateAround(FocusPoint(), transform.right, medialMovement * Time.deltaTime);
            }
        }
        else
        {
            if (goingUp)
            {
                var upMovementSpeed = -45f;
                _camera.transform.RotateAround(FocusPoint(), transform.right, upMovementSpeed * Time.deltaTime);
            }
            else if (goingDown)
            {
                var downMovementSpeed = 45f;
                _camera.transform.RotateAround(FocusPoint(), transform.right, downMovementSpeed * Time.deltaTime);
            }
        }

        if (RotatingOutsideLimit())
        {
            _camera.transform.position = previousPosition;
            _camera.transform.rotation = previousRotation;
        }

        var isPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
        if (!isPointerOverGameObject)
        {
            HandleZoom();
        }
    }

    private bool RotatingOutsideLimit()
    {
        var maxTilt = 80f;

        var cameraRelativePositive = _camera.transform.position - FocusPoint();
        var upwardsAngleReference = Vector3.up * cameraRelativePositive.magnitude;
        var right = Vector3.Angle(upwardsAngleReference, cameraRelativePositive);
        var adjustedAngles = right - 90f;
        return adjustedAngles < -maxTilt || adjustedAngles > maxTilt;
    }

    private void HandleZoom()
    {
        var minZoom = _zoomedOut ? MinZoomedOutDistance :
            CurrentPlanetController.Get().IsShipSelected() ? MinShipZoomedInDistance : MinZoomedInDistance;
        var maxZoom = _zoomedOut ? MaxZoomedOutDistance : MaxZoomedInDistance;
        var cameraTransform = _camera.transform;

        var scrollDelta = Input.mouseScrollDelta.y * .5f;
        var isScrolling = Math.Abs(scrollDelta) > 0.001f;

        var scrollSpeed = (_zoomedOut ? ZoomedOutSpeed : ZoomedInSpeed);
        var keySpeed = (_zoomedOut ? ZoomedOutSpeed * .5f : ZoomedInSpeed) * 4f;
        var speed = isScrolling ? scrollSpeed : keySpeed;

        var keyDelta = (Input.GetKey(KeyCode.S) ? -1f : Input.GetKey(KeyCode.W) ? 1f : 0f) * Time.deltaTime;
        var delta = isScrolling ? scrollDelta : keyDelta;

        if (delta != 0f)
        {
            var newPosition = cameraTransform.position + cameraTransform.forward * (delta * speed);
            var scrollDistance = Vector3.Distance(FocusPoint(), newPosition);

            if (_zoomedOut)
            {
                if (_hitLimit && scrollDistance < minZoom)
                {
                    ZoomIn();
                    var (position, rotation) = GetCameraZoomToDistance(MaxZoomedInDistance);
                    cameraTransform.position = position;
                    cameraTransform.rotation = rotation;
                    _hitLimit = false;
                }
                else
                {
                    _hitLimit = scrollDistance < minZoom;
                    if (scrollDistance <= maxZoom && scrollDistance >= minZoom)
                    {
                        cameraTransform.position = newPosition;
                    }
                }
            }
            else
            {
                if (_hitLimit && scrollDistance > maxZoom)
                {
                    ZoomOut();
                    var (position, rotation) = GetCameraZoomToDistance(MinZoomedOutDistance);
                    cameraTransform.position = position;
                    cameraTransform.rotation = rotation;
                    _hitLimit = false;
                }
                else
                {
                    _hitLimit = scrollDistance > maxZoom;
                    if (scrollDistance <= maxZoom && scrollDistance >= minZoom)
                    {
                        cameraTransform.position = newPosition;
                    }
                }
            }
        }
    }

    private void AbortMoveAndToggleZoom()
    {
        ClampAndFinishMove();
        ToggleZoomMode();
    }

    private void ProgressMove()
    {
        var linearProgress = Mathf.Clamp(_moveTime / _moveLength, 0f, 1f);
        var progress = _displayController.inputMode == DisplayController.InputMode.Cinematic
            ? EaseInOutCubic(linearProgress)
            : EaseOutCubic(linearProgress);
        transform.rotation = Quaternion.Slerp(_startRotation, _targetRotation, progress);
        // transform.rotation = Quaternion.Lerp(_startRotation, _targetRotation, progress);
        transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);

        _moveTime = Mathf.Min(_moveLength, _moveTime + Time.deltaTime);
    }

    private void ClampAndFinishMove()
    {
        var cameraTransform = transform;
        cameraTransform.rotation = _targetRotation;
        cameraTransform.position = _targetPosition;

        SetMoveFinished();
    }

    public void ToggleZoomMode()
    {
        _zoomedOut = !_zoomedOut;
        if (_zoomedOut)
        {
            PopupManager.Get().CancelAllPopups();

            var (targetPosition, targetRotation) = CameraPlanetZoomedOutPosition();
            var cameraTransform = _camera.transform;
            cameraTransform.position = targetPosition;
            cameraTransform.rotation = targetRotation;

            // Game start sequence finished
            if (_displayController.inputMode == DisplayController.InputMode.MapAndInventoryOnly)
            {
                _displayController.SetToStaticMode();
            }
        }
        else
        {
            PopupManager.Get().CancelAllPopups();

            var (targetPosition, targetRotation) = CameraPlanetZoomedInPosition();
            var cameraTransform = _camera.transform;
            cameraTransform.position = targetPosition;
            cameraTransform.rotation = targetRotation;
        }

        OnToggleZoom?.Invoke(_zoomedOut);
    }

    private void FocusOnStartingShip()
    {
        CurrentPlanetController.Get().FocusOnShip(startingShip.GetConvoyBeacon());
        _displayController.SetShipInFocus(startingShip.GetConvoyBeacon());

        _following = false;

        var previousFocusPoint = _focus ? _focus.position : _backupFocus;
        _focus = startingShip.transform;

        var cameraTransform = _camera.transform;
        _startPosition = cameraTransform.position;
        _startRotation = cameraTransform.rotation;
        (_targetPosition, _targetRotation) = CameraPlanetFocusPosition(previousFocusPoint, _focus.position);

        SetMoveStarted();

        _displayController.SetToCinematicMode();
        _moveLength = cinematicOpening ? 8f : .1f;
        _moveTime = 0f;

        _movingToShip = true;
    }

    public void EnterShip()
    {
        _following = false;
        _enteringShip = true;

        _focus = startingShip.transform;

        var cameraTransform = _camera.transform;
        _startPosition = cameraTransform.position;
        _startRotation = cameraTransform.rotation;

        _targetPosition = _focus.position;
        var direction = (_startPosition - _targetPosition).normalized;
        _targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        SetMoveStarted();

        _displayController.SetToCinematicMode();
        _moveLength = 3.5f;
        _moveTime = 0f;

        startingShip.StartHiding();
    }

    private void FinishedEnteringShip()
    {
        var notification = new TextNotification
        {
            Message = "Message from management: \"We left you some things to get started with.\"",
            TimeoutOverride = 20f,
        };
        Notifications.Get().Send(notification);

        DisplayController.Get().SetEnteredShip();
    }

    private void FocusOnPlanetSlowly(TinyPlanet planet)
    {
        FocusOnPlanet(planet);
        _displayController.SetToCinematicMode();
        _moveLength = cinematicOpening ? 8f : .1f;
        _moveTime = 0f;
    }

    public void FocusOnPlanet(TinyPlanet planet)
    {
        _following = false;

        _displayController.SetPlanetInFocus(planet);

        var center = TinyPlanetCenterPointHelper.GetMostCentralBlock(planet.Network().network);
        var previousFocusPoint = _focus ? _focus.position : _backupFocus;
        _focus = center.transform;

        var cameraTransform = _camera.transform;
        _startPosition = cameraTransform.position;
        _startRotation = cameraTransform.rotation;
        (_targetPosition, _targetRotation) = CameraPlanetFocusPosition(previousFocusPoint, _focus.position);

        var distance = (_targetPosition - _startPosition).magnitude;

        _moveLength = Mathf.Max(.25f, distance / 500f);
        _moveTime = 0f;

        SetMoveStarted();
    }

    private void SetMoveStarted()
    {
        _moving = true;
        OnNavigationStarted?.Invoke();
    }

    private void SetMoveFinished()
    {
        _moving = false;
        OnNavigationFinished?.Invoke();
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
            var newPosition = newFocusPoint + Vector3.forward * ZoomedInDistance;
            var direction = (newFocusPoint - newPosition).normalized;
            return new Tuple<Vector3, Quaternion>(newPosition,
                Quaternion.LookRotation(direction, Vector3.up));
        }
    }

    private Tuple<Vector3, Quaternion> GetCameraZoomToDistance(float distance)
    {
        var cameraTransform = _camera.transform;
        var cameraPosition = cameraTransform.position;

        var distanceFromCenter = Vector3.Distance(FocusPoint(), cameraPosition);
        var distanceToMove = distance - distanceFromCenter;
        var targetPosition = cameraPosition + cameraTransform.forward * -distanceToMove;

        return new Tuple<Vector3, Quaternion>(targetPosition, cameraTransform.rotation);
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

        var currentDistanceFromCenter = Vector3.Distance(FocusPoint(), cameraPosition);
        var targetDistanceFromCenter =
            CurrentPlanetController.Get().IsShipSelected() ? ShipZoomedInDistance : ZoomedInDistance;
        var distanceToMove = currentDistanceFromCenter - targetDistanceFromCenter;
        var targetPosition = cameraPosition + cameraTransform.forward * distanceToMove;

        return new Tuple<Vector3, Quaternion>(targetPosition, cameraTransform.rotation);
    }

    public bool IsZoomedOut()
    {
        return _zoomedOut;
    }

    private float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    private float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }

    public void ZoomIn()
    {
        if (IsZoomedOut()) ToggleZoomMode();
    }

    public void ZoomOut()
    {
        if (!IsZoomedOut()) ToggleZoomMode();
    }

    public bool ControlsLocked()
    {
        return _locked;
    }
}