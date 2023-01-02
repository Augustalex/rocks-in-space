using System;
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
        RouteEditor.Get().RouteFinished += RouteFinished;
    }

    private void RouteFinished(TinyPlanet start, TinyPlanet end)
    {
        if (end == _planet || start == _planet)
        {
            // TryAddLine(start, end);
            RefreshLines();
        }
    }

    private void RefreshLines()
    {
        ClearRouteLines();
        AddRouteLines();
    }

    private void TryAddLine(TinyPlanet start, TinyPlanet end)
    {
        var planetIsSameAsStart = _planet.planetId.Is(start.planetId);
        if (!planetIsSameAsStart) return;

        var line = Instantiate(PrefabTemplateLibrary.Get().routeLineTemplate);
        var lineController = line.GetComponent<RouteLine>();

        var (inboundOrNull, outboundOrNull) = RouteManager.Get().GetRoutesBetween(start, end);
        lineController.LinkBetween(_planet, end, inboundOrNull != null, HasPriority(inboundOrNull, outboundOrNull));

        _lines.Add(line);
    }

    private bool HasPriority(Route inboundOrNull, Route outboundOrNull)
    {
        if (inboundOrNull == null) return false;
        if (outboundOrNull == null) return false;

        return outboundOrNull.Order() > inboundOrNull.Order();
    }

    private void OnToggleZoom(bool zoomOn)
    {
        if (zoomOn)
        {
            AddRouteLines();
        }
        else
        {
            ClearRouteLines();
        }
    }

    private void ClearRouteLines()
    {
        foreach (var line in _lines)
        {
            Destroy(line);
        }

        _lines.Clear();
    }

    private void AddRouteLines()
    {
        foreach (var planetRoute in RouteManager.Get().GetPlanetRoutes(_planet))
        {
            var destinationPlanet = PlanetsRegistry.Get().FindPlanetById(planetRoute.destinationPlanetId);
            if (!destinationPlanet) continue;

            TryAddLine(_planet, destinationPlanet);
        }
    }

    public void MouseDown()
    {
        RouteEditor.Get().SelectRouteStart(_planet);
    }

    public void MouseUp()
    {
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