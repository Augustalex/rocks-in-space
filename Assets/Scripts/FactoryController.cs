using System.Collections;
using UnityEngine;

public class FactoryController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private const float InverseRate = 1f; // Smaller is faster
    private const int MetalsPerGadget = 10;

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

            var metals = _planetResources.GetMetals();
            if (metals >= MetalsPerGadget)
            {
                _planetResources.RemoveMetals(MetalsPerGadget);
                _planetResources.AddGadgets(1);
            }
        }
    }
}