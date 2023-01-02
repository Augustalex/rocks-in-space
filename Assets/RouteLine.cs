using UnityEngine;

public class RouteLine : MonoBehaviour
{
    public GameObject linePivot;
    public GameObject arrowPivot;

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
}