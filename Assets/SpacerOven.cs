using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacerOven : MonoBehaviour
{
    public void SetHeat(float factor)
    {
        var clampedFactor = Mathf.Clamp(factor, 0f, 1f);
        var material = GetComponent<MeshRenderer>().materials[0];
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", new Color(1.0f, 0, 0, 1.0f) * clampedFactor);
    }

    public void ResetHeat()
    {
        var material = GetComponent<MeshRenderer>().materials[0];
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", new Color(1.0f,0, 0,1.0f) * 0f);
        material.DisableKeyword("_EMISSION");
    }
}
