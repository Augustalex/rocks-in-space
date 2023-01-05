using UnityEngine;

public class PlanetLandmark : MonoBehaviour
{
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

    void Awake()
    {
        _planet = GetComponentInParent<TinyPlanet>();
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        _cameraController = CameraController.Get();
        _cameraController.OnToggleZoom += OnToggleZoom;

        _currentPlanetController = CurrentPlanetController.Get();
        _currentPlanetController.ShipSelected += OnShipSelected;

        UpdatePosition();
        Hide();
    }

    private void OnShipSelected(ColonyShip ship)
    {
        Show();
    }

    private void OnToggleZoom(bool zoomOn)
    {
        if (zoomOn)
        {
            ShowAndUpdatePosition();
        }
        else
        {
            Hide();
        }
    }

    public void Hover()
    {
        if (_planet.HasPort()) HandleHoverPopup();
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
                _planet.GetPort().GetPopupTarget().Show();
                _lastHovered = Time.time;
            }
        }
    }

    public void MouseDown()
    {
        if (_cameraController.IsZoomedOut()) RouteEditor.Get().SelectRouteStart(_planet);
    }

    public void MouseUp()
    {
        var routeEditor = RouteEditor.Get();

        if (_cameraController.IsZoomedOut() && routeEditor.IsValidDestination(_planet))
        {
            routeEditor.SelectRouteDestination(_planet);
        }
        else
        {
            NavigateToPlanet(_planet);
        }
    }

    private void ShowAndUpdatePosition()
    {
        transform.position = _planet.GetCenter();
        Show();
    }

    private void UpdatePosition()
    {
        transform.position = _planet.GetCenter();
    }

    private void Show()
    {
        UpdateStyle();
        gameObject.SetActive(true);
    }

    private void UpdateStyle()
    {
        _material.SetInt(HasPort, _planet.HasPort() ? 1 : 0);
        _material.SetInt(IsSelected, IsCurrentPlanet(_planet) ? 1 : 0);
        var inMapView = _cameraController.IsZoomedOut();
        _material.SetInt(InMapView, inMapView ? 1 : 0);
    }

    private void Hide()
    {
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
            cameraController.FocusOnPlanet(planet);
        }
    }

    private static bool IsCurrentPlanet(TinyPlanet planet)
    {
        return CurrentPlanetController.Get().CurrentPlanet() == planet;
    }
}