using UnityEngine;
using Random = UnityEngine.Random;

public class OreVein : MonoBehaviour
{
    private int _resources = 0;

    public static readonly int OrePerBlock = 50;

    private void Start()
    {
        var pieces = GetComponentsInChildren<MeshRenderer>();
        var onCount = 0;
        foreach (var meshRenderer in pieces)
        {
            if (Random.value < .65f) meshRenderer.gameObject.SetActive(false);
            else onCount += 1;
        }

        if (onCount == 0)
        {
            pieces[Random.Range(0, pieces.Length)].gameObject.SetActive(true);
            onCount += 1;
        }

        _resources += onCount * OrePerBlock;
    }

    public int Collect()
    {
        var resources = _resources;
        _resources = 0;

        return resources;
    }
}