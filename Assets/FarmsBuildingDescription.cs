using System;
using UnityEngine;

[RequireComponent(typeof(BuildingDescription))]
[RequireComponent(typeof(FarmController))]
public class FarmsBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string Get() // TODO REMOVE
    {
        return "";
    }
}