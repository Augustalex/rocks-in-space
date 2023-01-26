using System;
using UnityEngine;

[Serializable]
public struct Sphere
{
    public Vector3 position;
    public float radius;
}

[Serializable]
public struct Box
{
    public Vector3 position;
    public float xzScale;
    public float yScale;
}

[Serializable]
public enum PlanetShapes
{
    HourGlass,
    Blob,
    Disc
}

public enum DirectionSelectionModes
{
    UseAll, // No random selection, use all directions.
    RollOnEachDirection, // Roll for every direction whether to build or not. Every direction can be selected, even none can be selected.
    ControlledRandomSelection // Select a min and max amount of directions, and roll the exact amount.
}

[CreateAssetMenu(fileName = "PlanetGenerationSettings", menuName = "PlanetGenerationSettings", order = 0)]
public class PlanetGenerationSettings : ScriptableObject
{
    public PlanetShapes planetShape;

    public float hourGlassSidewayToHeightFactor;

    public float maxDistance;
    
    public float maxDepth;

    public float emptySpaceChance;

    public float emptySpaceMinDepth;

    public float maxEmptySpaces;

    public DirectionSelectionModes directionSelectionMode;

    public float baseFillChance;

    public AnimationCurve fillProbability;

    public int minDirections;

    public int maxDirections;

    public Sphere[] spheres;
    public Box[] boxes;
}