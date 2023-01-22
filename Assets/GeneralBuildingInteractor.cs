using Interactors;
using UnityEngine;

public class GeneralBuildingInteractor : InteractorModule
{
    [SerializeField] private string interactorName;
    [SerializeField] private BuildingType buildingType;

    public BuildingType GetBuildingType()
    {
        return buildingType;
    }

    public override InteractorType GetInteractorType()
    {
        return InteractorType.GeneralBuilding;
    }

    public override string GetInteractorName()
    {
        return interactorName;
    }

    public override string GetInteractorShortDescription()
    {
        return $"Placing {GetInteractorName()}";
    }

    public override void Build(Block block, RaycastHit raycastHit)
    {
        ConsumeRequiredResources(block);

        var seed = block.Seed(template);
        SetSeedRefund(seed);

        ProgressManager.Get().Built(buildingType);
    }
}