using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerControlled : MonoBehaviour
{
    private GameObject _lightsOn;
    private GameObject _lightsOff;

    void Start()
    {
        _lightsOn = GetComponentInChildren<LightsOn>().gameObject;
        _lightsOff = GetComponentInChildren<LightsOff>().gameObject;
        
        PowerOn();
    }
    
    public void PowerOn()
    {
        _lightsOn.SetActive(true);
        _lightsOff.SetActive(false);
    }

    public void PowerOff()
    {
        _lightsOn.SetActive(false);
        _lightsOff.SetActive(true);
    }
}
