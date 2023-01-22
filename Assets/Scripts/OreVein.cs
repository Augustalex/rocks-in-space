using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreVein : MonoBehaviour
{
    private int _resources = 0;

    public static readonly int OrePerBlock = 1;

    public GameObject[] faces;
    private TinyPlanetResources.PlanetResourceType _resourceType;

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
                meshTransform.localScale = new Vector3(Random.Range(1f, 1.7f), Random.Range(1f, 1.7f),
                    Random.Range(1.5f, 2.2f));
                meshTransform.localPosition = new Vector3(Random.Range(-.32f, .32f), Random.Range(-.32f, .32f), .48f);
                meshTransform.localRotation = Quaternion.Euler(Random.Range(0, 8) * 45f, Random.Range(0, 8) * 45f,
                    Random.Range(0, 8) * 45f);
                // meshTransform.localRotation = Random.rotation;
            }
        }


        // var pieces = GetComponentsInChildren<MeshRenderer>();
        // foreach (var meshRenderer in pieces)
        // {
        //     if (Random.value < .4f) meshRenderer.gameObject.SetActive(false);
        //     else
        //     {
        //         var meshTransform = meshRenderer.gameObject.transform;
        //         var scale = meshTransform.localScale;
        //         meshTransform.localScale = new Vector3(Random.Range(.2f, .25f), Random.Range(.2f, .45f), scale.z);
        //         meshTransform.localPosition = new Vector3(Random.Range(-.15f, .15f), Random.Range(-.15f, .15f), .45f);
        //         meshTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 8) * 45f);
        //         onCount += 1;
        //     }
        // }
        //
        // if (onCount == 0)
        // {
        //     var randomPiece = pieces[Random.Range(0, pieces.Length)].gameObject;
        //     var meshTransform = randomPiece.transform;
        //     var scale = meshTransform.localScale;
        //     var uniformScale = Random.Range(.2f, .35f);
        //     meshTransform.localScale = new Vector3(uniformScale, uniformScale, scale.z);
        //     meshTransform.localPosition = new Vector3(Random.Range(-.15f, .15f), Random.Range(-.15f, .15f), .45f);
        //     meshTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0, 8) * 45f);
        //     randomPiece.SetActive(true);
        //     onCount += 1;
        // }
        //
        _resources += onCount * OrePerBlock;
    }

    public int Collect(TinyPlanetResources planetResources)
    {
        var amount = _resources;

        planetResources.AddResource(_resourceType, _resources);
        _resources = 0;

        return amount;
    }

    public TinyPlanetResources.PlanetResourceType GetResourceType()
    {
        return _resourceType;
    }
}