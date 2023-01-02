using UnityEngine;

public class PlanetLandmark : MonoBehaviour
{
    private TinyPlanet _planet;
    private float _lastHovered;
    private Material _material;
    private static readonly int HasPort = Shader.PropertyToID("_HasPort");

    private const float HoverTooltipThreshold = .8f;
    private const float HoverTooltipGracePeriod = 1f;

    void Awake()
    {
        _planet = GetComponentInParent<TinyPlanet>();
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Start()
    {
        CameraController.Get().OnToggleZoom += OnToggleZoom;
        Hide();
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
        RouteEditor.Get().SelectRouteStart(_planet);
    }

    public void MouseUp()
    {
        var routeEditor = RouteEditor.Get();

        if (routeEditor.IsValidDestination(_planet))
        {
            routeEditor.SelectRouteDestination(_planet);
        }
        else
        {
            NavigateToPlanet(_planet);
        }
    }

    private void OnToggleZoom(bool zoomOn)
    {
        if (zoomOn)
        {
            ShowLandmark();
        }
        else
        {
            Hide();
        }
    }

    private void ShowLandmark()
    {
        _material.SetInt(HasPort, _planet.HasPort() ? 1 : 0);
        transform.position = _planet.GetCenter();
        gameObject.SetActive(true);
    }

    private void ShowPortLandmark()
    {
        _material.SetInt(HasPort, 1);
        transform.position = _planet.GetCenter();
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void NavigateToPlanet(TinyPlanet planet)
    {
        var cameraController = CameraController.Get();

        if (CurrentPlanetController.Get().CurrentPlanet() == planet)
        {
            cameraController.ToggleZoomMode();
        }
        else
        {
            CurrentPlanetController.Get().ChangePlanet(planet);
            cameraController.FocusOnPlanet(planet);
        }
    }
}