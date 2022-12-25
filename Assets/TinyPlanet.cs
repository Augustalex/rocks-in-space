using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TinyPlanet : MonoBehaviour
{
    public string planetName = "Unnamed";
    public List<GameObject> network;
    private PortController _port;

    public List<GameObject> FindDislocatedRocks(List<GameObject> dislodgedNetwork)
    {
        return network.Where(item => item != null && !dislodgedNetwork.Contains(item)).ToList();
    }

    public void SetNetwork(List<GameObject> newNetwork)
    {
        var workingNetwork = newNetwork.Where(n => n != null).ToList();
        
        network = workingNetwork;
        foreach (var networkItem in workingNetwork)
        {
            networkItem.transform.SetParent(transform);
        }
        
        foreach (var networkObject in network)
        {
            var port = networkObject.GetComponentInChildren<PortController>();
            if (port)
            {
                _port = port;
            }
        }
    }

    public void AddToPlanet(GameObject block)
    {
        block.transform.SetParent(transform);
        network.Add(block);
    }

    public void CheckDislodgement(GameObject rock)
    {
        // var sampleNetwork = TinyPlanetNetworkHelper.GetNetworkFromRock(rock);
        // if (IsNetworkDislodged(sampleNetwork))
        // {
        //     TinyPlanetGenerator.Get().TurnNetworkIntoPlanet(sampleNetwork);
        // }
    }

    public bool IsNetworkDislodged(List<GameObject> sampleNetwork)
    {
        var currentPlanet = sampleNetwork[0].GetComponentInParent<TinyPlanet>();

        return sampleNetwork.Count != currentPlanet.network.Count;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void RemoveFromNetwork(GameObject block)
    {
        network.Remove(block);
        
        if (block.GetComponentInChildren<PortController>())
        {
            _port = null;
        }
    }

    public TinyPlanetResources GetResources()
    {
        return GetComponent<TinyPlanetResources>();
    }

    public Vector3 GetCenter()
    {
        return TinyPlanetCenterPointHelper.CalculateCenter(network);
    }

    public void AttachPort(PortController port)
    {
        _port = port;
    }

    public bool HasPort()
    {
        return _port != null;
    }
}