using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRouteLines : MonoBehaviour
{
    private TinyPlanet _planet;
    private readonly List<Tuple<RouteLine, Route>> _lines = new();

    void Awake()
    {
        _planet = GetComponentInParent<TinyPlanet>();
    }

    private void Start()
    {
        CameraController.Get().OnToggleZoom += OnToggleZoom;
        RouteEditor.Get().RouteFinished += RouteFinished;
    }

    private void Update()
    {
        if (_lines.Count > 0)
        {
            foreach (var (line, route) in _lines)
            {
                line.SetIsActive(route.IsActive());
            }
        }
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

    private void RouteFinished(TinyPlanet start, TinyPlanet end)
    {
        if (end == _planet || start == _planet)
        {
            RefreshLines();
        }
    }

    private void RefreshLines()
    {
        ClearRouteLines();
        AddRouteLines();
    }

    private void AddRouteLines()
    {
        foreach (var planetRoute in RouteManager.Get().GetPlanetRoutes(_planet))
        {
            var planetIsSameAsStart = _planet.planetId.Is(planetRoute.StartPlanetId);
            if (!planetIsSameAsStart) continue;

            TryAddLine(planetRoute, _planet);
        }
    }

    private void TryAddLine(Route planetRoute, TinyPlanet start)
    {
        var planetIsSameAsStart = _planet.planetId.Is(start.planetId);
        if (!planetIsSameAsStart) return;

        var line = Instantiate(PrefabTemplateLibrary.Get().routeLineTemplate);
        var lineController = line.GetComponent<RouteLine>();
        lineController.LinkBetween(planetRoute);

        var lineMemo = new Tuple<RouteLine, Route>(lineController, planetRoute);
        _lines.Add(lineMemo);
        lineController.Removed += DoRemove;

        void DoRemove()
        {
            _lines.Remove(lineMemo);
            lineController.Removed -= DoRemove;
        }
    }

    private void ClearRouteLines()
    {
        foreach (var (lineController, route) in _lines)
        {
            Destroy(lineController.gameObject);
        }

        _lines.Clear();
    }
}