using System;
using System.Collections.Generic;
using UnityEngine;

public class PortController : MonoBehaviour
{
    private static readonly List<PortController> Ports = new();
    private TinyPlanetResources _resources;

    private void Start()
    {
        RegisterPort();
    }

    public static PortController GetMainPort()
    {
        // To be replaced by a system where you call in colonists to certain planets.
        // In the mean time we just put colonists on the starting planet.

        if (Ports.Count == 0)
        {
            throw new Exception(
                "Trying to access main port when hasn't placed any yet, or perhaps it was destroyed somehow?");
        }
        
        return Ports[0];
    }

    public static bool PortExists()
    {
        // In the future this should take a specific PortController or identifier for that port, to check if it still exists.
        // Since ports can be destroyed by meteors and alike, it's important to know if it is still there once the colony ship arrives.
        // For now we just check whether the starting port exists or not.

        return Ports.Count > 0;
    }

    public TinyPlanetResources GetResources()
    {
        return _resources;
    }

    private void RegisterPort()
    {
        Ports.Add(this);
    }

    private void OnDestroy()
    {
        Ports.Remove(this);
    }

    public void AttachPlanetResources(TinyPlanetResources resources)
    {
        _resources = resources;
    }
}
