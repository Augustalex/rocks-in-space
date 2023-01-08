using System;
using UnityEngine;

public class RouteLine : MonoBehaviour
{
    public GameObject linePivot;
    public GameObject arrowPivot;

    public event Action Removed;

    private MeshRenderer _lineMeshRenderer;
    private MeshRenderer _arrowMeshRenderer;
    private Material _lineMaterial;
    private Material _arrowMaterial;
    private static readonly int Active = Shader.PropertyToID("_IsActive");
    private static readonly int ResourceType = Shader.PropertyToID("_ResourceType");
    private Route _route;

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
        _arrowMaterial.SetInt(Active,
            1); // Always setting this to 1 let's the arrow be a UI element to always show what resource a line is trading
    }

    private void SetResourceType(TinyPlanetResources.PlanetResourceType routeResourceType)
    {
        _lineMaterial.SetFloat(ResourceType, (int)routeResourceType);
        _arrowMaterial.SetFloat(ResourceType, (int)routeResourceType);
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

        _route.Removed += OnRouteRemoved;
    }

    public void LinkBetween(TinyPlanet start, TinyPlanet end, bool planetHasInboundFromSource, bool planetHasPriority)
    {
        var startPosition = start.GetCenter();
        transform.position = startPosition;

        var endPosition = end.GetCenter();
        var targetVector = endPosition - startPosition;
        var distance = targetVector.magnitude - 25f;

        linePivot.transform.LookAt(endPosition);

        if (planetHasInboundFromSource)
        {
            endPosition += linePivot.transform.up * (planetHasPriority ? -8f : 8f);
            linePivot.transform.LookAt(endPosition);
        }

        var scale = linePivot.transform.localScale;
        linePivot.transform.localScale = new Vector3(scale.x, scale.y, distance * .5f);

        arrowPivot.transform.rotation = linePivot.transform.rotation;
        arrowPivot.transform.position = endPosition - arrowPivot.transform.forward * 26f;
    }

    private bool HasPriority(Route inboundOrNull, Route outboundOrNull)
    {
        if (inboundOrNull == null) return false;
        if (outboundOrNull == null) return false;

        return outboundOrNull.Order() > inboundOrNull.Order();
    }

    public void RemoveLine()
    {
        _route.Remove(); // Eventually triggers OnRouteRemoved();
    }

    private void OnRouteRemoved()
    {
        ClearLineDisplay();
    }

    private void ClearLineDisplay()
    {
        if (gameObject == null)
        {
            Debug.LogError("Trying to clear line that has already been cleared.");
            return;
        }

        Removed?.Invoke();
        Destroy(gameObject);
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
}