using UnityEngine;

[RequireComponent(typeof(BuildingDescription))]
public class GenericBuildingDescription : MonoBehaviour, IBuildingDescription
{
    public string description;

    public string Get()
    {
        return description;
    }
}
