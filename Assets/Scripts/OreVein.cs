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
            _ => throw new ArgumentOutOfRangeException(nameof(resourceType), resourceType, null)
        };

        var onCount = 0;

        foreach (var face in faces)
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
                // meshTransform.localRotation = Random.rotation;
            }
        }

        _orePerBlock = _resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Iron => 1,
            TinyPlanetResources.PlanetResourceType.Graphite => 5,
            TinyPlanetResources.PlanetResourceType.Copper => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(_resourceType), _resourceType, null)
        };
        _debrisMultiplier = _resourceType switch
        {
            TinyPlanetResources.PlanetResourceType.Iron => 1,
            TinyPlanetResources.PlanetResourceType.Graphite => 2,
            TinyPlanetResources.PlanetResourceType.Copper => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(_resourceType), _resourceType, null)
        };

        _resources += onCount * _orePerBlock;
    }

    public int Collect(TinyPlanetResources planetResources)
    {
        var amount = _resources;

        planetResources.AddResource(_resourceType, _resources);
        _resources = 0;

        return (amount / _orePerBlock) * _debrisMultiplier;
    }

    public TinyPlanetResources.PlanetResourceType GetResourceType()
    {
        return _resourceType;
    }
}