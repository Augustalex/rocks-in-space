using UnityEngine;

public class FactoryController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private const double Rate = 1f;
    private const int MetalThreshold = 2;
    private const int GadgetGain = 1;
    private double _cooldown = 0;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
    }

    void Update()
    {
        if (_cooldown >= 1f)
        {
            _cooldown = 0;

            var metals = _planetResources.GetMetals();
            if (metals >= MetalThreshold)
            {
                _planetResources.RemoveMetals(MetalThreshold);
                _planetResources.AddGadgets(GadgetGain);
            }
        }
        else
        {
            _cooldown += Rate * Time.deltaTime;
        }
    }
}
