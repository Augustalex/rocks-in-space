using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TinyPlanetGeneratorHelper
{
    public enum GeneratorRockType
    {
        Rock,
        Empty
    }

    private readonly Dictionary<Vector3, GeneratorRockType> _rocks = new();
    private int _emptySpaces;
    private int _tries;

    public Vector3[] NewNetworkTemplate(TinyPlanet.RockType planetType,
        PlanetGenerationSettings planetGenerationSettings)
    {
        var generationOrigin = Vector3.zero;

        GeneratePlanet(generationOrigin, planetGenerationSettings);
        var result = _rocks
            .Where(entry => entry.Value != GeneratorRockType.Empty)
            .Select(entry => entry.Key)
            .ToArray();

        if (_tries > 4)
        {
            Debug.LogError(
                $"A glitch in the matrix is making some planets come out too small. This one was only {result.Length} blocks!");
            return result;
        }

        if (result.Length > 10) return result;

        _tries += 1;
        return NewNetworkTemplate(planetType, planetGenerationSettings);
    }

    private void GeneratePlanet(Vector3 position, PlanetGenerationSettings settings, int depth = 0)
    {
        if (depth > settings.maxDepth) return;
        if (_rocks.ContainsKey(position)) return;

        foreach (var sphere in settings.spheres)
        {
            var distance = Vector3.Distance(position, sphere.position);
            if (distance < sphere.radius)
            {
                return;
            }
        }

        foreach (var box in settings.boxes)
        {
            var xDistance = Mathf.Abs(position.x - box.position.x);
            var zDistance = Mathf.Abs(position.z - box.position.z);
            var yDistance = Mathf.Abs(position.y - box.position.y);
            if (xDistance <= box.xzScale && zDistance <= box.xzScale && yDistance < box.yScale)
            {
                return;
            }
        }

        CreateRock(position);

        var directions = new[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
        };

        var selection = settings.directionSelectionMode switch
        {
            DirectionSelectionModes.UseAll => directions,
            DirectionSelectionModes.RollOnEachDirection => GetRandomDirections(Random.Range(0, directions.Length)),
            _ => GetRandomDirections(Random.Range(settings.minDirections, settings.maxDirections))
        };
        if (settings.planetShape == PlanetShapes.HourGlass)
        {
            // foreach (var direction in GetRandomDirections(Random.Range(settings.minDirections, settings.maxDirections)))
            foreach (var direction in selection)
            {
                var newPosition = position + direction;
                if (_rocks.ContainsKey(newPosition)) continue;

                var y = newPosition.y;
                var chanceUp = 1f - Mathf.Clamp((y < 0 ? 0 : y / settings.maxDistance), 0f, 1f);
                var chanceDown = 1f - Mathf.Clamp(y > 0 ? 0 : -y / settings.maxDistance, 0f, 1f);
                var chanceSideways =
                    Mathf.Clamp(Mathf.Abs(y) / (settings.maxDistance * settings.hourGlassSidewayToHeightFactor), 0f,
                        1f);

                var generate = false;
                var roll = Random.value;
                if (direction == Vector3.up)
                {
                    if (roll < chanceUp) generate = true;
                }
                else if (direction == Vector3.down)
                {
                    if (roll < chanceDown) generate = true;
                }
                else
                {
                    if (roll < chanceSideways) generate = true;
                }

                if (generate)
                {
                    var canMarkEmpty = settings.maxEmptySpaces < 0 || _emptySpaces < settings.maxEmptySpaces;
                    var shouldMarkEmpty = Random.value <= settings.emptySpaceChance &&
                                          depth >= settings.emptySpaceMinDepth;
                    if (canMarkEmpty && shouldMarkEmpty)
                    {
                        OccupySpace(newPosition);
                    }
                    else
                    {
                        GeneratePlanet(newPosition, settings, depth + 1);
                    }
                }


                // var canMarkEmpty = settings.MaxEmptySpaces < 0 || _emptySpaces < settings.MaxEmptySpaces;
                // var shouldMarkEmpty = Random.value <= settings.EmptySpaceChance && depth >= settings.EmptySpaceMinDepth;
                // if (canMarkEmpty && shouldMarkEmpty)
                // {
                //     OccupySpace(newPosition);
                // }
                // else
                // {
                //     var baseFillChance = settings.BaseFillChance;
                //     var chanceMultiplierMaxDistance = settings.MaxDistanceFillChanceMultiplier;
                //     var distanceToMax = newPosition.magnitude / chanceMultiplierMaxDistance;
                //     var distanceProb = settings.FillProbability.Evaluate(Mathf.Clamp(distanceToMax, 0f, 1f));
                //     var fillChance = distanceProb + baseFillChance;
                //     var shouldFill = Random.value < fillChance;
                //
                //     if (shouldFill)
                //     {
                //         GeneratePlanet(newPosition, settings, depth + 1);
                //     }
                // }
            }
        }
        else if (settings.planetShape == PlanetShapes.Blob)
        {
            foreach (var direction in selection)
            {
                var newPosition = position + direction;
                if (_rocks.ContainsKey(newPosition)) continue;

                var canMarkEmpty = settings.maxEmptySpaces < 0 || _emptySpaces < settings.maxEmptySpaces;
                var shouldMarkEmpty = Random.value <= settings.emptySpaceChance && depth >= settings.emptySpaceMinDepth;
                if (canMarkEmpty && shouldMarkEmpty)
                {
                    OccupySpace(newPosition);
                }
                else
                {
                    var prob = 1f - Vector3.Distance(newPosition, Vector3.zero) / settings.maxDistance;
                    if (Random.value < prob)
                    {
                        GeneratePlanet(newPosition, settings, depth + 1);
                    }
                }
            }
        }
        else
        {
            foreach (var direction in selection)
            {
                var newPosition = position + direction;
                if (_rocks.ContainsKey(newPosition)) continue;

                var canMarkEmpty = settings.maxEmptySpaces < 0 || _emptySpaces < settings.maxEmptySpaces;
                var shouldMarkEmpty = Random.value <= settings.emptySpaceChance && depth >= settings.emptySpaceMinDepth;
                if (canMarkEmpty && shouldMarkEmpty)
                {
                    OccupySpace(newPosition);
                }
                else
                {
                    var baseFillChance = settings.baseFillChance;
                    var chanceMultiplierMaxDistance = settings.maxDistance;
                    var distanceToMax = newPosition.magnitude / chanceMultiplierMaxDistance;
                    var distanceProb = settings.fillProbability.Evaluate(Mathf.Clamp(distanceToMax, 0f, 1f));
                    var fillChance = distanceProb + baseFillChance;
                    var shouldFill = Random.value < fillChance;

                    if (shouldFill)
                    {
                        GeneratePlanet(newPosition, settings, depth + 1);
                    }
                }
            }
        }
    }

    private Vector3[] GetRandomDirections(int count)
    {
        List<Vector3> directions = new()
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
        };
        if (count == directions.Count) return directions.ToArray();

        var result = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            var randomIndex = Random.Range(0, directions.Count);
            result.Add(directions[randomIndex]);
            directions.RemoveAt(randomIndex);
        }

        return result.ToArray();
    }

    private void CreateRock(Vector3 position)
    {
        _rocks[position] = GeneratorRockType.Rock;
    }

    private void OccupySpace(Vector3 position)
    {
        _emptySpaces += 1;
        _rocks[position] = GeneratorRockType.Empty;
    }
}