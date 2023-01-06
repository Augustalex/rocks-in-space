using System;
using Interactors;
using UnityEngine;

public class GifManager : MonoBehaviour
{
    public Texture[] refineryFrames;
    public Texture[] factoryFrames;
    public Texture[] powerPlantFrames;
    public Texture[] farmDomeFrames;
    public Texture[] housingModuleFrames;
    public Texture[] platformFrames;
    public Texture[] portFrames;

    private static GifManager _instance;

    public static GifManager Get()
    {
        return _instance;
    }

    void Awake()
    {
        _instance = this;
    }

    public Texture[] FramesByBuildingType(BuildingType buildingType)
    {
        return buildingType switch
        {
            BuildingType.Port => portFrames,
            BuildingType.Refinery => refineryFrames,
            BuildingType.Factory => factoryFrames,
            BuildingType.PowerPlant => powerPlantFrames,
            BuildingType.FarmDome => farmDomeFrames,
            BuildingType.ResidentModule => housingModuleFrames,
            BuildingType.Platform => platformFrames,
            _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
        };
    }
}