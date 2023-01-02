using System;
using UnityEngine;
using UnityEngine.Serialization;

public class RunningResourceEffect : MonoBehaviour
{
    [FormerlySerializedAs("cashPerSecond")]
    public float cashPerMinute;

    private GlobalResources _globalResources;

    void Start()
    {
        _globalResources = GlobalResources.Get();
    }

    private void Update()
    {
        _globalResources.AddCash(cashPerMinute / 60f);
    }
}