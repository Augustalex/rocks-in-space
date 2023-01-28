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
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Iron, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Graphite, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Copper, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Metals, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Gadgets, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Ice, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Water, 1000);
            CurrentPlanetController.Get().CurrentPlanet().GetResources()
                .AddResource(TinyPlanetResources.PlanetResourceType.Refreshments, 1000);
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

        // if (Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     NotificationSounds.Get().Play(NotificationTypes.Alerting);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     NotificationSounds.Get().Play(NotificationTypes.Informative);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     NotificationSounds.Get().Play(NotificationTypes.Positive);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     NotificationSounds.Get().Play(NotificationTypes.Negative);
        // }
    }
}