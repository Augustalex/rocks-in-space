using Interactors;
using UnityEngine;

public class PlanetLandmark : MonoBehaviour
{
    private const bool ShowInGotoMode = false;

    private TinyPlanet _planet;
    private float _lastHovered;
    private Material _material;
    private static readonly int HasPort = Shader.PropertyToID("_HasPort");
    private static readonly int IsSelected = Shader.PropertyToID("_IsSelected");
    private static readonly int InMapView = Shader.PropertyToID("_InMapView");
    private CameraController _cameraController;
    private CurrentPlanetController _currentPlanetController;

    private const float HoverTooltipThreshold = .8f;
    private const float HoverTooltipGracePeriod = 1f;

    public MapEffect iceMapEffect;

    private static PlanetId _contextDownOn = null;
    private static float _contextLastDown;

    void Awake()
    {
        _planet = GetComponentInParent<TinyPlanet>();
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        _cameraController = CameraController.Get();
        _cameraController.OnToggleZoom += (_) => UpdateDisplayState();
        _cameraController.OnNavigationStarted += OnCameraStartedMoving;
        _cameraController.OnNavigationFinished += UpdateDisplayState;

        _currentPlanetController = CurrentPlanetController.Get();
        _currentPlanetController.CurrentPlanetChanged += (_) => UpdateDisplayState();
        _currentPlanetController.ShipSelected += (_) => UpdateDisplayState();

        InteractorController.Get().InteractorSelected += (_) => UpdateDisplayState();

        UpdatePosition();
        Hide();
    }

    private void OnCameraStartedMoving()
    {
        // While moving between planets when not zoomed in, we don't want the landmarks bubble to appear
        // while the players camera is still on that planet. Since it looks bad!
        if (!_cameraController.IsZoomedOut()) Hide();
    }

    private void UpdateDisplayState()
    {
        var zoomedOut = _cameraController.IsZoomedOut();

        if (zoomedOut)
        {
            ShowAndUpdatePosition(true);
        }
        else
        {
            Hide();
        }
    }

    public void Hover()
    {
        if (_planet.HasPort() && !RouteEditor.Get().IsEditing())
        {
            HandleHoverPopup();
        }
    }

    private void HandleHoverPopup()
    {
        var timeSinceLastHovered = Time.time - _lastHovered;
        if (timeSinceLastHovered > HoverTooltipThreshold)
        {
            if (timeSinceLastHovered > (HoverTooltipThreshold + HoverTooltipGracePeriod))
            {
                _lastHovered = Time.time;
            }
            else
            {
                if (_planet.HasPort())
                {
                    _planet.GetPort().GetPopupTarget().Show();
                    _lastHovered = Time.time;
                }
            }
        }
    }

    public void MouseDown()
    {
        if (_cameraController.IsZoomedOut() && !RouteEditor.Get().IsEditing())
            RouteEditor.Get().StartCreatingRouteFrom(_planet);
    }

    public void ContextDown()
    {
        if (_cameraController.IsZoomedOut())
        {
            _contextDownOn = _planet.PlanetId;
            _contextLastDown = Time.time;
        }
    }

    public void ContextUp()
    {
        if (Time.time - _contextLastDown > 1f) ClearContextState();
        else if (_contextDownOn == _planet.PlanetId && _cameraController.IsZoomedOut())
        {
            PlayerShipManager.Get().ShipMover().MoveToPlanet(_planet);
        }
    }

    private void ClearContextState()
    {
        _contextLastDown = -1f;
        _contextDownOn = null;
    }

    public void MouseUp()
    {
        if (_cameraController.IsZoomedOut())
        {
            var routeEditor = RouteEditor.Get();
            if (routeEditor.IsIdle())
            {
                if (PlayerShipManager.Get().CurrentPlanet() == _planet.PlanetId || _planet.HasPort())
                {
                    NavigateToPlanet(_planet);
                }
            }
            else if (routeEditor.IsCreating())
            {
                if (routeEditor.IsValidDestination(_planet))
                {
                    routeEditor.SelectRouteDestination(_planet);
                }
                else
                {
                    routeEditor.Cancel();
                    NavigateToPlanet(_planet);
                }
            }
        }
    }

    private void ShowAndUpdatePosition(bool showMarker)
    {
        transform.position = _planet.Network().GetCenter();

        if (showMarker)
        {
            if (_planet.IsIcePlanet())
            {
                iceMapEffect.gameObject.SetActive(true);
            }
        }

        gameObject.SetActive(true);

        UpdateStyle(showMarker);
    }

    private void UpdatePosition()
    {
        var newPosition = _planet.Network().GetCenter();
        transform.position = newPosition;
        iceMapEffect.transform.position = newPosition;
    }

    private void UpdateStyle(bool showMarker)
    {
        _material.SetInt(HasPort,
            (PlayerShipOnPlanet() || _planet.HasPort()) ? 1 : 0);
        _material.SetInt(IsSelected, IsCurrentPlanet(_planet) ? 1 : 0);
        _material.SetInt(InMapView, showMarker ? 1 : 0);
    }

    private bool PlayerShipOnPlanet()
    {
        var playerShipManager = PlayerShipManager.Get();
        var currentPlanet = playerShipManager.CurrentPlanet();
        if (currentPlanet == null) return false;

        return currentPlanet.Is(_planet.PlanetId);
    }

    private void Hide()
    {
        iceMapEffect.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void NavigateToPlanet(TinyPlanet planet)
    {
        var cameraController = CameraController.Get();

        if (_cameraController.IsZoomedOut() && IsCurrentPlanet(planet))
        {
            cameraController.ToggleZoomMode();
        }
        else
        {
            CurrentPlanetController.Get().ChangePlanet(planet);
        }
    }

    private static bool IsCurrentPlanet(TinyPlanet planet)
    {
        return CurrentPlanetController.Get().CurrentPlanet() == planet;
    }
}