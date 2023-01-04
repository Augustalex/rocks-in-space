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
        switch (buildingType)
        {
            case BuildingType.Port:
                return portFrames;
                break;
            case BuildingType.Refinery:
                return refineryFrames;
                break;
            case BuildingType.Factory:
                return factoryFrames;
                break;
            case BuildingType.PowerPlant:
                return powerPlantFrames;
                break;
            case BuildingType.FarmDome:
                return farmDomeFrames;
                break;
            case BuildingType.ResidentModule:
                return housingModuleFrames;
                break;
            case BuildingType.Platform:
                return platformFrames;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null);
        }
    }
}