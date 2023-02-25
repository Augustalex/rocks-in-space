using System;
using System.Collections.Generic;
using UnityEngine;

public class RouteLine : MonoBehaviour
{
    public GameObject linePivot;
    public GameObject arrowPivot;

    public event Action RouteRemoved;

    private MeshRenderer _lineMeshRenderer;
    private MeshRenderer _arrowMeshRenderer;
    private Material _lineMaterial;
    private Material _arrowMaterial;
    private static readonly int Active = Shader.PropertyToID("_IsActive");
    private static readonly int ColorType = Shader.PropertyToID("_Color");
    private Route _route;

    private static readonly Dictionary<TinyPlanetResources.PlanetResourceType, Color> Colors =
        new()
        {
            { TinyPlanetResources.PlanetResourceType.Ore, new Color(1f, 0.845528f, 0f) },
            { TinyPlanetResources.PlanetResourceType.IronOre, new Color(1f, 1f, 1f) },
            { TinyPlanetResources.PlanetResourceType.Graphite, new Color(.05f, 0.05f, .2f) },
            { TinyPlanetResources.PlanetResourceType.CopperOre, new Color(1f, .4f, .2f) },
            { TinyPlanetResources.PlanetResourceType.IronPlates, new Color(.5f, 0.5f, .5f) },
            { TinyPlanetResources.PlanetResourceType.CopperPlates, new Color(.5f, .2f, .1f) },
            { TinyPlanetResources.PlanetResourceType.Gadgets, new Color(0.7359977f, 0f, 1f) },
            { TinyPlanetResources.PlanetResourceType.Ice, new Color(.75f, .75f, 1f) },
            { TinyPlanetResources.PlanetResourceType.Water, new Color(.2f, .2f, 1f) },
            { TinyPlanetResources.PlanetResourceType.Refreshments, new Color(0f, 0f, 1f) },
        };

    private void Awake()
    {
        _lineMeshRenderer = linePivot.GetComponentInChildren<MeshRenderer>();
        _arrowMeshRenderer = arrowPivot.GetComponentInChildren<MeshRenderer>();

        _lineMaterial = _lineMeshRenderer.material;
        _arrowMaterial = _arrowMeshRenderer.material;

        SetIsActive(false);
    }

    public void SetIsActive(bool isActive)
    {
        _lineMaterial.SetInt(Active, isActive ? 1 : 0);
        _arrowMaterial.SetInt(Active, isActive ? 1 : 0);
        // 1); // Always setting this to 1 let's the arrow be a UI element to always show what resource a line is trading
    }

    public void LinkBetween(Route route)
    {
        _route = route;
        var planetRegistry = PlanetsRegistry.Get();
        var start = planetRegistry.FindPlanetById(route.StartPlanetId);
        if (start == null) return;

        var destination = planetRegistry.FindPlanetById(route.DestinationPlanetId);
        if (destination == null) return;

        var (inboundOrNull, outboundOrNull) = RouteManager.Get().GetRoutesBetween(start, destination);
        LinkBetween(start, destination, inboundOrNull != null, HasPriority(inboundOrNull, outboundOrNull));
        SetIsActive(route.IsActive());

        SetResourceType(route.ResourceType);
    }

    private void SetResourceType(TinyPlanetResources.PlanetResourceType routeResourceType)
    {
        _lineMaterial.SetColor(ColorType, Colors[routeResourceType]);
        _arrowMaterial.SetColor(ColorType, Colors[routeResourceType]);
    }

    public void LinkBetween(TinyPlanet start, TinyPlanet end, bool planetHasInboundFromSource, bool planetHasPriority)
    {
        var startPosition = start.GetCenter();
        transform.position = startPosition;

        var endPosition = end.GetCenter();
        linePivot.transform.LookAt(endPosition);
        var startOffset = 6f;
        linePivot.transform.position +=
            linePivot.transform.forward *
            startOffset; // Move the line slightly away from the planet (so it is not confused with the planets hitbox)

        if (planetHasInboundFromSource)
        {
            endPosition += linePivot.transform.up * (planetHasPriority ? -8f : 8f);
            linePivot.transform.LookAt(endPosition);
        }

        var scale = linePivot.transform.localScale;
        var targetVector = endPosition - startPosition;
        var arrowHeadLineEndOffset =
            33f; // This ensures the line ends inside the arrow head mesh, but doesn't pass through it
        var distance = targetVector.magnitude - (arrowHeadLineEndOffset + startOffset);
        linePivot.transform.localScale = new Vector3(scale.x, scale.y, distance * .5f);

        arrowPivot.transform.rotation = linePivot.transform.rotation;
        var
            arrowHeadOffset =
                34f; // This offset ensures the arrowhead ends at an appropriate distance away from the planet, so it looks like it is pointing towards it.
        arrowPivot.transform.position = endPosition - arrowPivot.transform.forward * arrowHeadOffset;
    }

    private bool HasPriority(Route inboundOrNull, Route outboundOrNull)
    {
        if (inboundOrNull == null) return false;
        if (outboundOrNull == null) return false;

        return outboundOrNull.Order() > inboundOrNull.Order();
    }

    public void RemoveLine()
    {
        RouteRemoved?.Invoke();
    }

    public void EditRoute()
    {
        var planetRegistry = PlanetsRegistry.Get();
        var start = planetRegistry.FindPlanetById(_route.StartPlanetId);
        if (start == null)
        {
            RemoveLine();
            return;
        }

        var destination = planetRegistry.FindPlanetById(_route.DestinationPlanetId);
        if (destination == null)
        {
            RemoveLine();
            return;
        }

        RouteEditor.Get().EditRoute(start, destination);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}