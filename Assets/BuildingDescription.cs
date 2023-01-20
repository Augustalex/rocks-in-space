using UnityEngine;

public interface IBuildingDescription
{
    public string Get();
}

public class BuildingDescription : MonoBehaviour
{
    public string GetDescription()
    {
        var buildingDescription = GetComponent<IBuildingDescription>();

        return buildingDescription.Get();
    }
}