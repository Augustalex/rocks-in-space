using System;
using GameNotifications;
using Interactors;
using TMPro;
using UnityEngine;

public class V1ProgressLock : MonoBehaviour
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
                Hide();
                break;
            case BuildingType.Factory:
                Hide();
                break;
            case BuildingType.PowerPlant:
                PowerPlant();
                break;
            case BuildingType.FarmDome:
                FarmDome();
                break;
            case BuildingType.ResidentModule:
                HousingModule();
                break;
            case BuildingType.Platform:
                Platform();
                break;
            case BuildingType.Purifier:
                IceMachine(BuildingType.Purifier);
                break;
            case BuildingType.Distillery:
                IceMachine(BuildingType.Distillery);
                break;
            case BuildingType.KorvKiosk:
                // Hide();
                KorvKiosk();
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

    private void PowerPlant()
    {
        if (PowerPlantUnlocked())
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = $"New building \"{PowerPlantInteractor.GetName()}\" unlocked!",
                NotificationType = NotificationTypes.Positive
            });
            Hide();
        }
        else
        {
            HideVeil();
            _text.text = "Unlock with\n1 Refinery\n1 Factory";
        }
    }

    private void FarmDome()
    {
        if (HousingUnlocked())
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = $"New building \"{FarmDomeInteractor.GetName()}\" unlocked!",
                NotificationType = NotificationTypes.Positive
            });
            Hide();
        }
        else
        {
            if (PowerPlantUnlocked())
            {
                HideVeil();
                _text.text = "Unlock with\nPower Plant";
            }
            else
            {
                ShowVeil();
                _text.text = "???";
            }
        }
    }

    private void HousingModule()
    {
        if (HousingUnlocked())
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = $"New building \"{ResidencyInteractor.GetName()}\" unlocked!",
                NotificationType =
                    NotificationTypes
                        .Silent // Farm notification is shown at the same time, so no need to ring another notification sound.
            });
            Hide();
        }
        else
        {
            if (PowerPlantUnlocked())
            {
                HideVeil();
                _text.text = "Unlock with\nPower Plant";
            }
            else
            {
                ShowVeil();
                _text.text = "???";
            }
        }
    }

    private void Platform()
    {
        if (PlatformUnlocked())
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = $"New building \"{ScaffoldingInteractor.GetName()}\" unlocked!",
                NotificationType = NotificationTypes.Positive
            });
            Hide();
        }
        else
        {
            if (HousingUnlocked())
            {
                HideVeil();
                _text.text = "Unlock with\n500 colonists";
            }
            else
            {
                ShowVeil();
                _text.text = "???";
            }
        }
    }

    private void IceMachine(BuildingType buildingType)
    {
        if (buildingType != BuildingType.Distillery && buildingType != BuildingType.Purifier)
            throw new Exception("Wrong building type passed to IceMachine ProgressLock method!");
        
        if (IceMachinesUnlocked())
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = $"New building \"{InteractorController.Get().GetInteractorByBuildingType(buildingType).GetInteractorName()}\" unlocked!",
                NotificationType = NotificationTypes.Positive
            });
            Hide();
        }
        else
        {
            if (PlatformUnlocked())
            {
                HideVeil();
                _text.text = "Unlock with\n1000 colonists";
            }
            else
            {
                ShowVeil();
                _text.text = "???";
            }
        }
    }

    private void KorvKiosk()
    {
        var progressManager = ProgressManager.Get();
        if (progressManager.TotalColonistsCount() >=
            10000)
        {
            Notifications.Get().Send(new BuildingNotification
            {
                Message = $"Something different has been unlocked in the building menu.",
                NotificationType = NotificationTypes.Positive
            });
            Hide();
        }
        else
        {
            ShowVeil();

            if (PlatformUnlocked())
            {
                _text.text = "Unlock with\n10000 colonists";
            }
            else
            {
                ShowVeil();
                _text.text = "???";
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

    private bool PlatformUnlocked()
    {
        var progressManager = ProgressManager.Get();
        return
            progressManager.TotalColonistsCount() >=
            500; // TODO: Perhaps this unlocking checks should happen outside the UI!! Perhaps in an update loop in the ProgressManager?
    }

    private bool IceMachinesUnlocked()
    {
        var progressManager = ProgressManager.Get();
        return
            progressManager.TotalColonistsCount() >=
            1000; // TODO: Perhaps this unlocking checks should happen outside the UI!! Perhaps in an update loop in the ProgressManager?
    }

    private bool HousingUnlocked()
    {
        var progressManager = ProgressManager.Get();
        return progressManager.HasBuilt(BuildingType.PowerPlant);
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
}