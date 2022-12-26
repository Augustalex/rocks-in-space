using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    private GameObject _beam;

    void Start()
    {
        _beam = GetComponentInChildren<LaserBeam>().gameObject;
    }

    public void TurnOn(float length = 100f)
    {
        _beam.SetActive(true);

        var scaleFactor = length / 100;
        var currentScale = _beam.transform.localScale;
        _beam.transform.localScale = new Vector3(currentScale.x, currentScale.y, scaleFactor);
    }

    public void TurnOff()
    {
        _beam.SetActive(false);
    }
}
