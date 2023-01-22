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
                    ProgressManager.Get().Started(),
                    NotificationMessage(BuildingType.Refinery),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Platform:
                CheckLock(
                    ProgressManager.Get().Started(),
                    NotificationMessage(BuildingType.Platform),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Lander:
                CheckLock(
                    ProgressManager.Get().Started(),
                    NotificationMessage(BuildingType.Lander),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.ResidentModule:
                CheckLock(
                    ProgressManager.Get().Started(),
                    NotificationMessage(BuildingType.ResidentModule),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Factory:
                CheckLock(
                    ProgressManager.Get().Surviving(),
                    NotificationMessage(BuildingType.Factory),
                    SurvivingLockMessage(),
                    !ProgressManager.Get().Started()
                );
                break;
            case BuildingType.Purifier:
                CheckLock(
                    ProgressManager.Get().Surviving(),
                    NotificationMessage(BuildingType.Purifier),
                    SurvivingLockMessage(),
                    !ProgressManager.Get().Started()
                );
                break;
            case BuildingType.PowerPlant:
                CheckLock(
                    ProgressManager.Get().Surviving(),
                    NotificationMessage(BuildingType.PowerPlant),
                    SurvivingLockMessage(),
                    !ProgressManager.Get().Started()
                );
                break;
            case BuildingType.FarmDome:
                CheckLock(
                    ProgressManager.Get().Comfortable(),
                    NotificationMessage(BuildingType.FarmDome),
                    ComfortableLockMessage(),
                    !ProgressManager.Get().Surviving()
                );
                break;
            case BuildingType.Distillery:
                CheckLock(
                    ProgressManager.Get().Comfortable(),
                    NotificationMessage(BuildingType.Distillery),
                    ComfortableLockMessage(),
                    !ProgressManager.Get().Surviving()
                );
                break;
            case BuildingType.KorvKiosk:
                CheckLock(
                    ProgressManager.Get().Luxurious(),
                    "Something different has been unlocked in the building menu.",
                    LuxuriousLockMessage(),
                    !ProgressManager.Get().Comfortable()
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

    public string SurvivingLockMessage()
    {
        return "Unlock with\n500 colonists";
    }

    public string ComfortableLockMessage()
    {
        return "Unlock with\n2000 housed colonists";
    }

    public string LuxuriousLockMessage()
    {
        return "Unlock with\n10000 overjoyed colonists";
    }
}