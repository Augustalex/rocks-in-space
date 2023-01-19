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
        _planetAttachment.AttachedTo += OnResourceEffectAttached;
        _planetAttachment.TransferredFromTo += (from, to) =>
        {
            OnResourceEffectDetached(from);
            OnResourceEffectAttached(to);
        };
        _planetAttachment.DetachedFrom += OnResourceEffectDetached;
    }

    private void OnResourceEffectDetached(TinyPlanetResources resources)
    {
        resources.DeregisterBuilding(buildingType);
    }

    private void OnResourceEffectAttached(TinyPlanetResources resources)
    {
        resources.RegisterBuilding(buildingType);
    }
}
