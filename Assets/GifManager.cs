using System;
using Interactors;
using UnityEngine;

public class GifManager : MonoBehaviour
{
    public Texture[] refineryFrames;
    public Texture[] factoryFrames;
    public Texture[] solarPanelsFrames;
    public Texture[] proteinFabricatorFrames;
    public Texture[] powerPlantFrames;
    public Texture[] farmDomeFrames;
    public Texture[] housingModuleFrames;
    public Texture[] platformFrames;
    public Texture[] portFrames;
    public Texture[] purifierFrames;
    public Texture[] distilleryFrames;
    public Texture[] korvKioskFrames;

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
            BuildingType.Purifier => purifierFrames,
            BuildingType.Distillery => distilleryFrames,
            BuildingType.KorvKiosk => korvKioskFrames,
            BuildingType.SolarPanels => solarPanelsFrames,
            BuildingType.ProteinFabricator => proteinFabricatorFrames,
            _ => throw new ArgumentOutOfRangeException(nameof(buildingType), buildingType, null)
        };
    }
}