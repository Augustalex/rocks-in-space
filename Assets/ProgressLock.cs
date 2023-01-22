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
            case BuildingType.Refinery:
                CheckLock(
                    ProgressManager.Get().RefineryUnlocked(),
                    NotificationMessage(_buildingType),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Factory:
                CheckLock(
                    ProgressManager.Get().FactoryUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Refinery).GetInteractorName()}",
                    !ProgressManager.Get().RefineryUnlocked()
                );
                break;
            case BuildingType.SolarPanels:
                CheckLock(
                    ProgressManager.Get().ColonyBasicsProductionUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Factory).GetInteractorName()}",
                    !ProgressManager.Get().FactoryUnlocked()
                );
                break;
            case BuildingType.ResidentModule:
                CheckLock(
                    ProgressManager.Get().HousingUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.SolarPanels).GetInteractorName()}&\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.ProteinFabricator).GetInteractorName()}",
                    !ProgressManager.Get().ColonyBasicsProductionUnlocked()
                );
                break;
            case BuildingType.Platform:
                CheckLock(
                    ProgressManager.Get().HousingUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.SolarPanels).GetInteractorName()}&\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.ProteinFabricator).GetInteractorName()}",
                    !ProgressManager.Get().ColonyBasicsProductionUnlocked()
                );
                break;
            case BuildingType.Purifier:
                CheckLock(
                    ProgressManager.Get().IceProductionUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock with\n2000 happy colonists",
                    !ProgressManager.Get().HousingUnlocked()
                );
                break;
            case BuildingType.FarmDome:
                CheckLock(
                    ProgressManager.Get().LuxuryProductionUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Purifier).GetInteractorName()}",
                    !ProgressManager.Get().IceProductionUnlocked()
                );
                break;
            case BuildingType.Distillery:
                CheckLock(
                    ProgressManager.Get().LuxuryProductionUnlocked(),
                    NotificationMessage(_buildingType),
                    $"Unlock by building\n{InteractorController.Get().GetGenericInteractorByBuildingType(BuildingType.Purifier).GetInteractorName()}",
                    !ProgressManager.Get().IceProductionUnlocked()
                );
                break;
            case BuildingType.PowerPlant:
                CheckLock(
                    ProgressManager.Get().LuxuryProductionUnlocked(),
                    NotificationMessage(_buildingType),
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
        bool veiled = false)
    {
        if (unlocked)
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = unlockedMessage,
                NotificationType = NotificationTypes.Positive
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
            $"New building \"{InteractorController.Get().GetGenericInteractorByBuildingType(buildingType).GetInteractorName()}\" unlocked!";
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