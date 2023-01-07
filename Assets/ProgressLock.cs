using System;
using System.Collections;
using GameNotifications;
using Interactors;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProgressLock : MonoBehaviour
{
    private TMP_Text _text;
    private bool _hidden;
    private BuildingType _buildingType;

    private const float LoopDelay = 2f;

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
        StartCoroutine(UpdateLocksLoop());
    }

    private IEnumerator UpdateLocksLoop()
    {
        while (!_hidden && gameObject != null)
        {
            yield return new WaitForSeconds(LoopDelay);
            if (Random.value < .5f) yield return new WaitForNextFrameUnit();
            UpdateLocks();
        }
    }

    private void UpdateLocks()
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
            Notifications.Get().Send(new TextNotification
            {
                message = $"{PowerPlantInteractor.GetName()} building option unlocked!"
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
            Notifications.Get().Send(new TextNotification
            {
                message = $"{FarmDomeInteractor.GetName()} building option unlocked!"
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
            Notifications.Get().Send(new TextNotification
            {
                message = $"{ResidencyInteractor.GetName()} building option unlocked!"
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
        var progressManager = ProgressManager.Get();
        if (progressManager.TotalColonistsCount() >=
            1000) // TODO: Perhaps this unlocking checks should happen outside the UI!! Perhaps in an update loop in the ProgressManager?
        {
            Notifications.Get().Send(new TextNotification
            {
                message = $"{ScaffoldingInteractor.GetName()} building option unlocked!"
            });
            Hide();
        }
        else
        {
            if (HousingUnlocked())
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
}