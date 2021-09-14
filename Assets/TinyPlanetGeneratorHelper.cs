using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TinyPlanetGeneratorHelper
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
        GeneratePlanet(generationOrigin, Random.Range(5, 15));
        return _rocks
            .Where(entry => entry.Value != RockType.Empty)
            .Select(entry => entry.Key)
            .ToArray();
    }

    private void GeneratePlanet(Vector3 position, int maxDepth, int depth = 0)
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
            if (_rocks.ContainsKey(newPosition)) continue;

            var distanceProb = Mathf.Max(0.1f, newPosition.magnitude / 20f);
            
            if (Random.value < distanceProb + .4f)
            {
                GeneratePlanet(newPosition, maxDepth, depth + 1);
            }
            else
            {
                OccupySpace(newPosition);
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