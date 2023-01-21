using GameNotifications;
using Interactors;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public bool on;
    private DebugHidable[] _hidable;

    void Start()
    {
        if (on)
        {
            _hidable = FindObjectsOfType<DebugHidable>();
            // ProgressManager.Get().Built(BuildingType.Factory);
            // ProgressManager.Get().Built(BuildingType.Refinery);
            // ProgressManager.Get().Built(BuildingType.PowerPlant);
        }
    }

    void Update()
    {
        if (!on) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var debugHidable in _hidable)
            {
                debugHidable.gameObject.SetActive(!debugHidable.gameObject.activeSelf);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            CurrentPlanetController.Get().CurrentPlanet().GetResources().AddOre(1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources().AddMetals(1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources().AddGadgets(1000);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GlobalResources.Get().AddCash(10000);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            GlobalResources.Get().UseCash(100);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            CurrentPlanetController.Get().CurrentPlanet().GetResources().AddColonists(1000);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            MeteorGenerator.Get().SpawnOnCurrentPlanet();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            NotificationSounds.Get().Play(NotificationTypes.Alerting);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            NotificationSounds.Get().Play(NotificationTypes.Informative);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            NotificationSounds.Get().Play(NotificationTypes.Positive);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            NotificationSounds.Get().Play(NotificationTypes.Negative);
        }
    }
}