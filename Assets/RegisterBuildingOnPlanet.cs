using Interactors;
using UnityEngine;

[RequireComponent(typeof(AttachedToPlanet))]
public class RegisterBuildingOnPlanet : MonoBehaviour
{
    [SerializeField] private BuildingType buildingType;
    private AttachedToPlanet _planetAttachment;

    void Awake()
    {
        _planetAttachment = GetComponent<AttachedToPlanet>();
        _planetAttachment.AttachedTo += Attached;
        _planetAttachment.TransferredFromTo += (from, to) =>
        {
            Detached(from);
            Attached(to);
        };
        _planetAttachment.DetachedFrom += Detached;
    }

    private void Detached(TinyPlanetResources resources)
    {
        resources.DeregisterBuilding(buildingType);
    }

    private void Attached(TinyPlanetResources resources)
    {
        ProgressManager.Get().Built(buildingType);
        resources.RegisterBuilding(buildingType);
    }

    public BuildingType GetBuildingType()
    {
        return buildingType;
    }
}