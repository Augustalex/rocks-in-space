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
                    Started(),
                    NotificationMessage(BuildingType.Refinery),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Platform:
                CheckLock(
                    Started(),
                    NotificationMessage(BuildingType.Platform),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.ResidentModule:
                CheckLock(
                    Started(),
                    NotificationMessage(BuildingType.ResidentModule),
                    StartedLockMessage(),
                    false
                );
                break;
            case BuildingType.Factory:
                CheckLock(
                    Surviving(),
                    NotificationMessage(BuildingType.Factory),
                    SurvivingLockMessage(),
                    !Started()
                );
                break;
            case BuildingType.Purifier:
                CheckLock(
                    Surviving(),
                    NotificationMessage(BuildingType.Purifier),
                    SurvivingLockMessage(),
                    !Started()
                );
                break;
            case BuildingType.PowerPlant:
                CheckLock(
                    Surviving(),
                    NotificationMessage(BuildingType.PowerPlant),
                    SurvivingLockMessage(),
                    !Started()
                );
                break;
            case BuildingType.FarmDome:
                CheckLock(
                    Comfortable(),
                    NotificationMessage(BuildingType.FarmDome),
                    ComfortableLockMessage(),
                    !Surviving()
                );
                break;
            case BuildingType.Distillery:
                CheckLock(
                    Comfortable(),
                    NotificationMessage(BuildingType.Distillery),
                    ComfortableLockMessage(),
                    !Surviving()
                );
                break;
            case BuildingType.KorvKiosk:
                CheckLock(
                    Luxurious(),
                    "Something different has been unlocked in the building menu.",
                    LuxuriousLockMessage(),
                    !Comfortable()
                );
                break;
            default:
                throw new ArgumentOutOfRangeException();
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

    private bool PowerPlantUnlocked()
    {
        var progressManager = ProgressManager.Get();
        return progressManager.HasBuilt(BuildingType.Refinery) && progressManager.HasBuilt(BuildingType.Factory);
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

    public bool Started()
    {
        var progressManager = ProgressManager.Get();
        return progressManager.HasBuilt(BuildingType.Port);
    }

    public string StartedLockMessage()
    {
        return "Unlock by building\nyour first Beacon";
    }

    public bool Surviving()
    {
        var progressManager = ProgressManager.Get();
        return progressManager.GetColonistCount(PlanetColonistMonitor.PlanetStatus.Neutral) >= 500;
    }

    public string SurvivingLockMessage()
    {
        return "Unlock with\n500 colonists";
    }

    public bool Comfortable()
    {
        var progressManager = ProgressManager.Get();
        return progressManager.GetColonistCount(PlanetColonistMonitor.PlanetStatus.Happy) >= 2000;
    }

    public string ComfortableLockMessage()
    {
        return "Unlock with\n2000 happy colonists";
    }

    public bool Luxurious()
    {
        var progressManager = ProgressManager.Get();
        return progressManager.GetColonistCount(PlanetColonistMonitor.PlanetStatus.Overjoyed) >= 10000;
    }

    public string LuxuriousLockMessage()
    {
        return "Unlock with\n10000 overjoyed colonists";
    }
}