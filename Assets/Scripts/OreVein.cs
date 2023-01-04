using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class OreVein : MonoBehaviour
{
    private void Start()
    {
        var pieces = GetComponentsInChildren<MeshRenderer>();
        var onCount = 0;
        foreach (var meshRenderer in pieces)
        {
            if (Random.value < .6f) meshRenderer.gameObject.SetActive(false);
            else onCount += 1;
        }

        if (onCount == 0)
        {
            pieces[Random.Range(0, pieces.Length)].gameObject.SetActive(true);
        }
    }
}