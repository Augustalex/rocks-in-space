using UnityEngine;

public class RouteLine : MonoBehaviour
{
    public GameObject linePivot;
    public GameObject arrowPivot;

    public void LinkBetween(TinyPlanet start, TinyPlanet end)
    {
        var startPosition = start.GetCenter();
        transform.position = startPosition;

        var endPosition = end.GetCenter() + Vector3.up * 2f;
        var diff = endPosition - startPosition;
        var distance = diff.magnitude - 1f;

        var scale = linePivot.transform.localScale;
        linePivot.transform.LookAt(endPosition);
        linePivot.transform.localScale = new Vector3(scale.x, scale.y, distance * .5f);

        arrowPivot.transform.rotation = linePivot.transform.rotation;
        arrowPivot.transform.position = endPosition - arrowPivot.transform.right;
    }
}