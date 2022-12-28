using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TinyPlanetCenterPointHelper
{
    public static GameObject CalculateCenter(List<GameObject> network)
    {
        var centroid = Vector3.zero;
        
        foreach (var networkItem in network)
        {
            centroid = centroid + networkItem.transform.position;
        }
        centroid /= network.Count;

        var closestPoint = GetClosestPoint(centroid, network);

        return closestPoint;
    }
    
    public static GameObject GetMostCentralBlock(IEnumerable<GameObject> network)
    {
        var safeNetwork = network.Where(n => n != null);
        var centroid = Vector3.zero;
        var points = network.Where(n => n != null).Select(item => item.transform.position).ToArray();
        foreach (var vector in points)
        {
            centroid = centroid + vector;
        }
        centroid /= points.Length;

        var closestBlock = GetClosestBlock(centroid, safeNetwork);
        return closestBlock;
    }
    
    private static GameObject GetClosestPoint(Vector3 target, List<GameObject> network)
    {
        GameObject tMin = network[0];
        float minDist = Mathf.Infinity;
        foreach (var networkItem in network)
        {
            var position = networkItem.transform.position;
            float dist = Vector3.Distance(position, target);
            if (dist < minDist)
            {
                tMin = networkItem;
                minDist = dist;
            }
        }
        
        return tMin;
    }
    private static GameObject GetClosestBlock(Vector3 target, IEnumerable<GameObject> blocks)
    {
        GameObject closestBlock = null;
        float minDist = Mathf.Infinity;
        foreach (var block in blocks)
        {
            var position = block.transform.position;
            float dist = Vector3.Distance(position, target);
            if (dist < minDist)
            {
                closestBlock = block;
                minDist = dist;
            }
        }
        
        return closestBlock;
    }
}