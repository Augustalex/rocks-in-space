using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlanetGenerationMapping", menuName = "PlanetGenerationMapping", order = 0)]
public class PlanetGenerationMapping : ScriptableObject
{
    public PlanetGenerationSettings ice;
    public PlanetGenerationSettings blue;
    public PlanetGenerationSettings green;
    public PlanetGenerationSettings orange;

    public PlanetGenerationSettings Get(TinyPlanet.RockType rockType)
    {
        return rockType switch
        {
            TinyPlanet.RockType.Orange => orange,
            TinyPlanet.RockType.Blue => blue,
            TinyPlanet.RockType.Green => green,
            TinyPlanet.RockType.Ice => ice,
            _ => throw new ArgumentOutOfRangeException(nameof(rockType), rockType, null)
        };
    }
}