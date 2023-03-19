using System;
using GameNotifications;
using Interactors;
using TMPro;
using UnityEngine;

public class ProgressLock : MonoBehaviour
{
    private TMP_Text _text;
    private bool _hidden;
    private BuildingType _buildingType;

    public GameObject lightVeil;
    public GameObject textVeil;
    private GifDisplay _gifDisplay;

    private void Awake()
    {
        _text = GetComponentInChildren<TMP_Text>();
    }

    public void CheckLock(BuildingType buildingType)
    {
        _buildingType = buildingType;
        var gifDisplay = GetComponentInChildren<GifDisplay>();
        gifDisplay.frames = GifManager.Get().FramesByBuildingType(buildingType);
        _gifDisplay = gifDisplay;

        UpdateLocks();
    }

    public void UpdateLocks()
    {
        switch (_buildingType)
        {
            case BuildingType.Port:
                Hide();
                break;
            case BuildingType.Lander:
                CheckLock(
                    ProgressManager.Get().FirstPortBuilt(),
                    $"You can now place your {InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Lander).GetInteractorName()}!",
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.SolarPanels:
                CheckLock(
                    ProgressManager.Get().FirstPortBuilt(),
                    GenericUnlockMessage(),
                    // $"Unlock by producing\n{TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets)}",
                    // $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Factory).GetInteractorName()}",
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.ProteinFabricator:
                CheckLockWithoutNotification(
                    ProgressManager.Get().FirstPortBuilt(),
                    // $"Unlock by producing\n{TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets)}",
                    // $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Factory).GetInteractorName()}",
                    StartedLockMessage(),
                    !ProgressManager.Get().FactoryUnlocked()
                );
                break;
            case BuildingType.ResidentModule:
                CheckLockWithoutNotification(
                    ProgressManager.Get().FirstPortBuilt(),
                    // GenericUnlockMessage(),
                    // $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.SolarPanels).GetInteractorName()}&\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.ProteinFabricator).GetInteractorName()}",
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Refinery:
                CheckLock(
                    ProgressManager.Get().ColonyStarted(),
                    GenericUnlockMessage(),
                    $"Unlock with 200 happy colonists",
                    !ProgressManager.Get().FirstPortBuilt()
                );
                break;
            case BuildingType.CopperRefinery:
                CheckLockWithoutNotification(
                    ProgressManager.Get().ColonyStarted(),
                    // GenericUnlockMessage(),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Lander).GetInteractorName()}",
                    !ProgressManager.Get().FirstPortBuilt()
                );
                break;
            case BuildingType.Factory:
                CheckLockWithoutNotification(
                    ProgressManager.Get().ColonyStarted(),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Lander).GetInteractorName()}",
                    !ProgressManager.Get().FirstPortBuilt()
                );
                break;
            case BuildingType.Platform:
                CheckLockWithoutNotification(
                    ProgressManager.Get().IceProductionUnlocked(),
                    $"Unlock with\n2000 happy colonists",
                    // $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.SolarPanels).GetInteractorName()}&\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.ProteinFabricator).GetInteractorName()}",
                    !ProgressManager.Get().RefineryUnlocked()
                );
                break;
            case BuildingType.Purifier:
                CheckLock(
                    ProgressManager.Get().IceProductionUnlocked(),
                    GenericUnlockMessage(),
                    $"Unlock with\n2000 happy colonists",
                    !ProgressManager.Get().RefineryUnlocked()
                );
                break;
            case BuildingType.FarmDome:
                CheckLock(
                    ProgressManager.Get().LuxuryProductionUnlocked(),
                    GenericUnlockMessage(),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Purifier).GetInteractorName()}",
                    !ProgressManager.Get().IceProductionUnlocked()
                );
                break;
            case BuildingType.Distillery:
                CheckLockWithoutNotification(
                    ProgressManager.Get().LuxuryProductionUnlocked(),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Purifier).GetInteractorName()}",
                    !ProgressManager.Get().IceProductionUnlocked()
                );
                break;
            case BuildingType.PowerPlant:
                CheckLockWithoutNotification(
                    ProgressManager.Get().LuxuryProductionUnlocked(),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Purifier).GetInteractorName()}",
                    !ProgressManager.Get().IceProductionUnlocked()
                );
                break;
            case BuildingType.KorvKiosk:
                CheckLock(
                    ProgressManager.Get().EndGameReached(),
                    "Something different has been unlocked in the building menu.",
                    LuxuriousLockMessage(),
                    !ProgressManager.Get().LuxuryProductionUnlocked()
                );
                break;
            default:
                Hide();
                break;
        }
    }

    private void Hide()
    {
        _hidden = true;
        Destroy(gameObject);
    }

    private void CheckLock(bool unlocked = false, string unlockedMessage = "", string lockMessage = "",
        bool veiled = false, bool silent = false)
    {
        if (unlocked)
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = unlockedMessage,
                NotificationType = silent ? NotificationTypes.Silent : NotificationTypes.Positive
            });
            Hide();
        }
        else
        {
            if (veiled)
            {
                ShowVeil();
                _text.text = "???";
            }
            else
            {
                HideVeil();
                _text.text = lockMessage;
            }
        }
    }

    private void CheckLockWithoutNotification(bool unlocked = false, string lockMessage = "",
        bool veiled = false)
    {
        if (unlocked)
        {
            Hide();
        }
        else
        {
            if (veiled)
            {
                ShowVeil();
                _text.text = "???";
            }
            else
            {
                HideVeil();
                _text.text = lockMessage;
            }
        }
    }

    private void HideVeil()
    {
        lightVeil.SetActive(true);

        _gifDisplay.RemoveShade();
        textVeil.SetActive(false);
    }

    private void ShowVeil()
    {
        lightVeil.SetActive(false);

        _gifDisplay.Shade();
        textVeil.SetActive(true);
    }

    public bool Hidden()
    {
        return _hidden;
    }

    private string NotificationMessage(BuildingType buildingType)
    {
        return
            $"New building {InteractorController.Get().GetGenericInteractorByBuildingType(buildingType).GetInteractorName()} unlocked!";
    }

    private string GenericUnlockMessage()
    {
        return
            $"New buildings options unlocked!";
    }

    public string StartedLockMessage()
    {
        return "Unlock by building\nyour first Beacon";
    }

    public string LuxuriousLockMessage()
    {
        return "Unlock with\n10000 overjoyed colonists";
    }
}