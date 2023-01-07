using System.Collections;
using UnityEngine;

public class RefineryController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private const float InverseRate = 2f; // Smaller is faster
    private const int OrePerMetal = 10;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
        StartCoroutine(Check());
    }

    private IEnumerator Check()
    {
        while (gameObject != null)
        {
            yield return new WaitForSeconds(InverseRate);

            var ore = _planetResources.GetOre();
            if (ore >= OrePerMetal)
            {
                _planetResources.RemoveOre(OrePerMetal);
                _planetResources.AddMetals(1);
            }
        }
    }
}