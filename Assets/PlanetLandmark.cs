using System.Collections.Generic;
using UnityEngine;

public class PlanetLandmark : MonoBehaviour
{
    private TinyPlanet _planet;
    private readonly List<GameObject> _lines = new();

    void Awake()
    {
        _planet = GetComponentInParent<TinyPlanet>();
    }

    private void Start()
    {
        CameraController.Get().OnToggleZoom += OnToggleZoom;
        RouteEditor.Get().RouteFinished += TryAddLine;
    }

    private void TryAddLine(TinyPlanet start, TinyPlanet end)
    {
        var startPlanetName = start.planetName;
        var endPlanetName = end.planetName;
        var planetIsSameAsStart = _planet.planetId.Is(start.planetId);
        var planetName = _planet.planetName;
        Debug.Log("TRY ADD LINE:" + startPlanetName + "->" + endPlanetName + ", but planet is actually: " + planetName + " - ARE THE STARTS THE SAME? " + !planetIsSameAsStart);
        if (!planetIsSameAsStart) return;
        Debug.Log("YES! Adding line.");

        var line = Instantiate(PrefabTemplateLibrary.Get().routeLineTemplate);
        var lineController = line.GetComponent<RouteLine>();

        lineController.LinkBetween(_planet, end);

        _lines.Add(line);
    }

    private void OnToggleZoom(bool zoomOn)
    {
        if (zoomOn)
        {
            foreach (var planetRoute in RouteManager.Get().GetPlanetRoutes(_planet))
            {
                var destinationPlanet = PlanetsRegistry.Get().FindPlanetById(planetRoute.destinationPlanetId);
                if (!destinationPlanet) continue;

                TryAddLine(_planet, destinationPlanet);
            }
        }
        else
        {
            foreach (var line in _lines)
            {
                Destroy(line);
            }

            _lines.Clear();
        }
    }

    public void MouseDown()
    {
        Debug.Log("MOUSE DOWN ON: " + _planet.planetName);
        RouteEditor.Get().SelectRouteStart(_planet);
    }

    public void MouseUp()
    {
        Debug.Log("MOUSE UP ON: " + _planet.planetName);
        if (RouteEditor.Get().IsValidDestination(_planet))
        {
            RouteEditor.Get().SelectRouteDestination(_planet);
        }
        else
        {
            NavigateToPlanet(_planet);
        }
    }

    public void Hover()
    {
        var routeEditor = RouteEditor.Get();
        if (routeEditor.IsEditing() && routeEditor.IsValidDestination(_planet))
        {
            // attach live link
        }
    }

    private void NavigateToPlanet(TinyPlanet planet)
    {
        var cameraController = CameraController.Get();

        if (CurrentPlanetController.Get().CurrentPlanet() == planet) return;

        CurrentPlanetController.Get().ChangePlanet(planet);
        cameraController.FocusOnPlanet(planet);
    }
}