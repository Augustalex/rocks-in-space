using UnityEngine;

public class RefineryController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private const double Rate = 2f;
    private const int OrePerMetal = 5;
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

            var ore = _planetResources.GetOre();
            if (ore >= OrePerMetal)
            {
                _planetResources.SetOre(ore - OrePerMetal);
                _planetResources.SetMetals(_planetResources.GetMetals() + 1);
            }
        }
        else
        {
            _cooldown += Rate * Time.deltaTime;
        }
    }
}
