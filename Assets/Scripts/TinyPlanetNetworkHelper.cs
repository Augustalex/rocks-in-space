using System;
using System.Collections.Generic;
using UnityEngine;

public static class TinyPlanetNetworkHelper
{
    public static float NetworkJointDistance = .8f;
    public static float NetworkDislodgeActivationDistance = .8f;
    
    public static List<GameObject> GetNetworkFromRock(GameObject rock)
    {
        var rockPosition = rock.transform.position;
        var positionRockPairs = new List<GameObject> {rock};
        AddNearbyRocks(rockPosition, positionRockPairs);

        return positionRockPairs;
    }

    private static void AddNearbyRocks(
        Vector3 position,
        List<GameObject> alreadyFound
    )
    {
        var hits = Physics.OverlapSphere(position, NetworkJointDistance);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var block = hit.GetComponent<Block>();
                if (block)
                {
                    var blockRoot = block.GetRoot();
                    var blockAlreadyFound = alreadyFound.Exists(item => item == blockRoot);
                    if (!blockAlreadyFound)
                    {
                        alreadyFound.Add(blockRoot);

                        var blockPosition = block.transform.position;
                        AddNearbyRocks(blockPosition, alreadyFound);
                    }
                }
            }
        }
    }
}