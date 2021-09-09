using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TinyPlanetCenterPointHelper
{
    public static Vector3 CalculateCenter(List<GameObject> network)
    {
        var centroid = Vector3.zero;
        var points = network.Select(item => item.transform.position).ToArray();
        foreach (var vector in points)
        {
            centroid = centroid + vector;
        }
        centroid /= points.Length;

        var closestPoint = GetClosestPoint(centroid, points);

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
    
    private static Vector3 GetClosestPoint(Vector3 target, Vector3[] points)
    {
        Vector3 tMin = Vector3.zero;
        float minDist = Mathf.Infinity;
        foreach (var position in points)
        {
            float dist = Vector3.Distance(position, target);
            if (dist < minDist)
            {
                tMin = position;
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