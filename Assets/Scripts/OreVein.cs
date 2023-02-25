using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreVein : MonoBehaviour
{
    private int _resources = 0;

    public GameObject[] faces;
    private TinyPlanetResources.PlanetResourceType _resourceType;
    private int _orePerBlock;
    private int _debrisMultiplier;

    public void Setup(TinyPlanetResources.PlanetResourceType resourceType)
    {
        _resourceType = resourceType;
        var template = resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Ore => PrefabTemplateLibrary.Get().ironOre,
            TinyPlanetResources.PlanetResourceType.Iron => PrefabTemplateLibrary.Get().ironOre,
            TinyPlanetResources.PlanetResourceType.Graphite => PrefabTemplateLibrary.Get().graphiteOre,
            TinyPlanetResources.PlanetResourceType.Copper => PrefabTemplateLibrary.Get().copperOre,
            TinyPlanetResources.PlanetResourceType.Dangeronium => PrefabTemplateLibrary.Get().dangeroniumOre,
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
        };

        var onCount = 0;

        var horizontalFaces = faces[..4];
        var verticalFaces = faces[4..6];

        foreach (var face in horizontalFaces)
        {
            var faceCount = Random.Range(0, Random.value < .2f ? 8 : 4);
            for (int i = 0; i < faceCount; i++)
            {
                onCount += 1;
                var ore = Instantiate(template, face.transform);
                var meshTransform = ore.transform;
                var scale = meshTransform.localScale;
                if (resourceType == TinyPlanetResources.PlanetResourceType.Graphite)
                {
                    var newScale = Random.Range(1f, 1.1f);
                    meshTransform.localScale = new Vector3(newScale, newScale, newScale);
                }
                else
                {
                    meshTransform.localScale = new Vector3(Random.Range(1f, 1.7f), Random.Range(1f, 1.7f),
                        Random.Range(1.5f, 2.2f));
                }

                meshTransform.localPosition = new Vector3(Random.Range(-.32f, .32f), Random.Range(-.32f, .32f), .48f);
                meshTransform.localRotation = Quaternion.Euler(Random.Range(0, 8) * 45f, Random.Range(0, 8) * 45f,
                    Random.Range(0, 8) * 45f);
            }
        }

        foreach (var face in verticalFaces)
        {
            var oreCount = Random.value < .5f ? 0 : Random.Range(1, 4);
            for (int i = 0; i < oreCount; i++)
            {
                onCount += 1;
                var ore = Instantiate(template, face.transform);
                var meshTransform = ore.transform;
                var scale = meshTransform.localScale;
                if (resourceType == TinyPlanetResources.PlanetResourceType.Graphite)
                {
                    var newScale = Random.Range(1f, 1.1f);
                    meshTransform.localScale = new Vector3(newScale, newScale, newScale);
                }
                else
                {
                    meshTransform.localScale = new Vector3(Random.Range(1f, 1.7f), Random.Range(1f, 1.7f),
                        Random.Range(1.5f, 2.2f));
                }

                meshTransform.localPosition = new Vector3(Random.Range(-.32f, .32f), Random.Range(-.32f, .32f), .48f);
                meshTransform.localRotation = Quaternion.Euler(Random.Range(0, 8) * 45f, Random.Range(0, 8) * 45f,
                    Random.Range(0, 8) * 45f);
            }
        }

        _orePerBlock = _resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Iron => 2,
            TinyPlanetResources.PlanetResourceType.Graphite => 5,
            TinyPlanetResources.PlanetResourceType.Copper => 2,
            TinyPlanetResources.PlanetResourceType.Dangeronium => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(_resourceType), _resourceType, null)
        };
        _debrisMultiplier = _resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Iron => 1,
            TinyPlanetResources.PlanetResourceType.Graphite => 1,
            TinyPlanetResources.PlanetResourceType.Copper => 1,
            TinyPlanetResources.PlanetResourceType.Dangeronium => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(_resourceType), _resourceType, null)
        };

        _resources += onCount * _orePerBlock;
    }

    public void CollectResources(TinyPlanetResources planetResources)
    {
        planetResources.AddResource(_resourceType, _resources);
        _resources = 0;
    }
    
    public void Clear()
    {
        _resources = 0;
    }

    public int DebrisCount()
    {
        var amount = _resources;
        return (amount / _orePerBlock) * _debrisMultiplier;
    }
    
    public TinyPlanetResources.PlanetResourceType GetResourceType()
    {
        return _resourceType;
    }
}