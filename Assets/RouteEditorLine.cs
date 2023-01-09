using System;
using UnityEngine;

public class RouteEditorLine : MonoBehaviour
{
    public GameObject line;

    private RouteEditor _routeEditor;
    private TinyPlanet _start;
    private TinyPlanet _end;
    private float _showUntil = -1f;
    private float _startedAt;

    private static RouteEditorLine _instance;

    public static RouteEditorLine Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _routeEditor = RouteEditor.Get();

        _routeEditor.RouteStarted += RouteStarted;
        _routeEditor.RouteDestinationSelected += RouteFinished;
        _routeEditor.RouteCancelled += RouteCancelled;
    }

    private void RouteCancelled()
    {
        ResetAndHideLine();
    }

    private void RouteFinished()
    {
        ResetAndHideLine();
    }

    private void RouteStarted(TinyPlanet start)
    {
        if (_start)
        {
            ResetAndHideLine();
        }

        _startedAt = Time.time;
        _start = start;
    }

    void Update()
    {
        if (_showUntil > 0 && Time.time > _showUntil)
        {
            ResetAndHideLine();
            return;
        }

        if (_start != null)
        {
            var passedCooldown = _startedAt < 0 || Time.time - _startedAt > .5f;
            var movedPassedThreshold = line.transform.localScale.x > 5f;
            if (!passedCooldown && !movedPassedThreshold)
            {
                if (line.activeSelf) line.SetActive(false);
            }
            else
            {
                if (!line.activeSelf) line.SetActive(true);

                var mainCamera = CameraController.GetCamera();
                var startPosition = RectTransformUtility.WorldToScreenPoint(mainCamera, _start.GetCenter());
                var mousePosition = (Vector2)Input.mousePosition;
                var currentPosition =
                    _end
                        ? RectTransformUtility.WorldToScreenPoint(mainCamera, _end.GetCenter())
                        : mousePosition;

                LinkBetween(startPosition, currentPosition);
            }
        }
    }

    private void LinkBetween(Vector2 start, Vector2 end)
    {
        line.transform.position = start;

        var diff = end - start;
        var distance = diff.magnitude;

        var scale = line.transform.localScale;
        line.transform.LookAt(end);
        line.transform.right = end - start;
        line.transform.localScale = new Vector3(distance, scale.y, scale.z);
    }

    private void ResetAndHideLine()
    {
        Reset();
        line.SetActive(false);
    }

    private void Reset()
    {
        _start = null;
        _end = null;
        _showUntil = -1f;
        _startedAt = -1f;
    }

    public bool Drawing()
    {
        return _start != null || _end != null;
    }
}