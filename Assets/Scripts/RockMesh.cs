using System;
using UnityEngine;

public class RockMesh : MonoBehaviour
{
    [SerializeField] private SpriteRenderer billboardRenderer;
    
    public Material orange;
    public Material blue;
    public Material green;
    public Material ice;
    
    public void RefreshMaterial(TinyPlanet.RockType rockType)
    {
        billboardRenderer.material = rockType switch
        {
            TinyPlanet.RockType.Orange => orange,
            TinyPlanet.RockType.Blue => blue,
            TinyPlanet.RockType.Green => green,
            TinyPlanet.RockType.Snow => ice,
            TinyPlanet.RockType.Ice => ice,
            _ => throw new ArgumentOutOfRangeException(nameof(rockType), rockType, null)
        };
    }
}