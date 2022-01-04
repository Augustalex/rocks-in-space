using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class LargePlanetGeneratorHelper
{
    public enum RockType
    {
        Rock,
        Empty
    }

    private readonly Dictionary<Vector3, RockType> _rocks = new Dictionary<Vector3, RockType>();

    public Vector3[] NewNetworkTemplate()
    {
        var generationOrigin = Vector3.zero;
        GeneratePlanet(generationOrigin, 50, generationOrigin);
        return _rocks
            .Where(entry => entry.Value != RockType.Empty)
            .Select(entry => entry.Key)
            .ToArray();
    }

    private void GeneratePlanet(Vector3 position, int maxDepth, Vector3 originPosition, int depth = 0)
    {
        if (depth > maxDepth) return;
        if (_rocks.ContainsKey(position)) return;
        CreateRock(position);

        var directions = new []
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
        };
        foreach (var direction in directions)
        {
            var newPosition = position + direction;
            // if (_rocks.ContainsKey(newPosition)) continue;

            var distanceProb = Mathf.Clamp(Vector3.Distance(originPosition, newPosition) / 20f, 0.1f, 1f);
            Debug.Log(distanceProb);
            if (distanceProb > .5f)
            {
                OccupySpace(newPosition);
            }
            else
            {
                GeneratePlanet(newPosition, maxDepth, originPosition, depth + 1);
            }
        }
    }      
    
    private void CreateRock(Vector3 position)
    {
        _rocks[position] = RockType.Rock;
    }

    private void OccupySpace(Vector3 position)
    {
        _rocks[position] = RockType.Empty;
    }

}