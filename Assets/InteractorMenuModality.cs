using System;
using Interactors;
using UnityEngine;

public class InteractorMenuModality : MonoBehaviour
{
    public static InteractorCategory GetCategoryFromInteractorType(InteractorType interactorType)
    {
        switch (interactorType)
        {
            case InteractorType.Dig:
                return InteractorCategory.Dig;
            case InteractorType.Port:
                return InteractorCategory.Build;
            case InteractorType.Refinery:
                return InteractorCategory.Build;
            case InteractorType.Factory:
                return InteractorCategory.Build;
            case InteractorType.PowerPlant:
                return InteractorCategory.Build;
            case InteractorType.FarmDome:
                return InteractorCategory.Build;
            case InteractorType.ResidentModule:
                return InteractorCategory.Build;
            case InteractorType.Platform:
                return InteractorCategory.Build;
            case InteractorType.Select:
                return InteractorCategory.Select;
            case InteractorType.KorvKiosk:
                return InteractorCategory.Build;
            case InteractorType.Misc:
                return InteractorCategory.Dig;
            default:
                throw new ArgumentOutOfRangeException(nameof(interactorType), interactorType, null);
        }
    }

    private static InteractorMenuModality _instance;

    public static InteractorMenuModality Get()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
}