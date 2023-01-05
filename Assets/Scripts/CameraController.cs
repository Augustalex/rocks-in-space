using System;
using System.Collections;
using Interactors;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float ZoomedOutDistance = 600f;
    private const float MinZoomedOutDistance = 400f;
    private const float MaxZoomedOutDistance = 1200f;
    private const float ZoomedOutSpeed = 320f;

    private const float ZoomedInDistance = 32f;
    private const float MinZoomedInDistance = 18f;
    private const float MaxZoomedInDistance = 60f;
    private const float ZoomedInSpeed = 10f;

    private const float ShipZoomedInDistance = 60f;

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
    private static bool _hasInstance;
    private bool _zoomedOut;
    private MapPopupTarget _currentTarget;

    public event Action<bool> OnToggleZoom;
    public event Action OnNavigationStarted;

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
            if (_moveTime < _moveLength)
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
            else
            {
                var cameraTransform = transform;
                cameraTransform.rotation = _targetRotation;
                cameraTransform.position = _targetPosition;
                _moving = false;
                if (_displayController.inputMode == DisplayController.InputMode.Cinematic)
                    _displayController.ExitCinematicMode();
            }
        }
        else if (_displayController.inputMode == DisplayController.InputMode.Static)
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

            var minZoom = _zoomedOut ? MinZoomedOutDistance : MinZoomedInDistance;
            var maxZoom = _zoomedOut ? MaxZoomedOutDistance : MaxZoomedInDistance;
            var cameraTransform = _camera.transform;
            var distance = Vector3.Distance(FocusPoint(), cameraTransform.position);

            var speed = _zoomedOut ? ZoomedOutSpeed : ZoomedInSpeed;
            if (Input.GetKey(KeyCode.S) && distance < maxZoom)
            {
                cameraTransform.position += cameraTransform.forward * (-speed * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.W) && distance > minZoom)
            {
                cameraTransform.position += cameraTransform.forward * (speed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleZoomMode();
            }
        }
    }

    public void ToggleZoomMode()
    {
        _zoomedOut = !_zoomedOut;
        if (_zoomedOut)
        {
            InteractorController.Get().LockToDefaultInteractor();
            PopupManager.Get().CancelAllPopups();

            var (targetPosition, targetRotation) = CameraPlanetZoomedOutPosition();
            var cameraTransform = _camera.transform;
            cameraTransform.position = targetPosition;
            cameraTransform.rotation = targetRotation;
        }
        else
        {
            InteractorController.Get().UnlockFromDefaultInteractor();
            PopupManager.Get().CancelAllPopups();

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
        _moveLength = cinematicOpening ? 8f : .1f;
        _moveTime = 0f;
    }

    public void FocusOnShip(ColonyShip colonyShip)
    {
        _moving = true;
        _following = false;

        _displayController.SetShipInFocus(colonyShip);

        var center = colonyShip.GetCenterGo();
        var previousFocusPoint = _focus ? _focus.position : _backupFocus;
        _focus = center.transform;

        var cameraTransform = _camera.transform;
        _startPosition = cameraTransform.position;
        _startRotation = cameraTransform.rotation;
        (_targetPosition, _targetRotation) = CameraPlanetFocusPosition(previousFocusPoint, _focus.position);

        var distance = (_targetPosition - _startPosition).magnitude;

        _moveLength = Mathf.Max(.25f, distance / 500f);
        _moveTime = 0f;

        OnNavigationStarted?.Invoke();
    }

    public void FocusOnPlanet(TinyPlanet planet)
    {
        _moving = true;
        _following = false;

        _displayController.SetPlanetInFocus(planet);

        var center = TinyPlanetCenterPointHelper.GetMostCentralBlock(planet.network);
        var previousFocusPoint = _focus ? _focus.position : _backupFocus;
        _focus = center.transform;

        var cameraTransform = _camera.transform;
        _startPosition = cameraTransform.position;
        _startRotation = cameraTransform.rotation;
        (_targetPosition, _targetRotation) = CameraPlanetFocusPosition(previousFocusPoint, _focus.position);

        var distance = (_targetPosition - _startPosition).magnitude;

        _moveLength = Mathf.Max(.25f, distance / 500f);
        _moveTime = 0f;

        OnNavigationStarted?.Invoke();
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
}