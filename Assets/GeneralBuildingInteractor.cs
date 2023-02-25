using GameNotifications;
using Interactors;
using UnityEngine;

public class GeneralBuildingInteractor : InteractorModule
{
    [SerializeField] private string interactorName;
    [SerializeField] private BuildingType buildingType;
    private bool _sentCopperBonusHint;

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

        if (buildingType == BuildingType.CopperRefinery && block.GetConnectedPlanet().GetRockType().IsCopper())
        {
            if (!_sentCopperBonusHint)
            {
                _sentCopperBonusHint = true;
                Notifications.Get().Send(new TextNotification
                {
                    Message =
                        "Your new Copper refineries seems to be almost twice as effective on this kind of asteroid! Producing twice as many plates for each ore."
                });
            }
        }
    }
}