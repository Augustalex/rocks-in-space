using UnityEngine;

public class FarmController : MonoBehaviour
{
    private TinyPlanetResources _planetResources;
    private const float FoodPerSecond = 10f;

    void Start()
    {
        _planetResources = GetComponentInParent<TinyPlanetResources>();
    }

    void Update()
    {
        var energy = _planetResources.GetEnergy();
        if (energy >= 0)
        {
            _planetResources.AddFood(FoodPerSecond * Time.deltaTime);
        }
    }
}