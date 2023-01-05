using System;
using System.Collections;
using UnityEngine;

namespace GameNotifications
{
    public class PlanetResourceMonitor : MonoBehaviour
    {
        private TinyPlanet _planet;
        private TinyPlanetResources _resources;
        private TinyPlanetResources.ResourcesData _previousResources;

        private void Awake()
        {
            _planet = GetComponent<TinyPlanet>();
            _resources = _planet.GetResources();
            _previousResources = _resources.CopyData();
        }

        private void Start()
        {
            StartCoroutine(CheckLoop());
        }

        IEnumerator CheckLoop()
        {
            while (gameObject != null)
            {
                yield return new WaitForSeconds(2f);
                Check();
            }
        }

        private void Check()
        {
            var newData = _resources.CopyData();
            if (Math.Abs(newData.Energy - _previousResources.Energy) > .5f)
            {
                if (newData.Inhabitants > 0)
                {
                    if (newData.Energy <= 0f)
                    {
                        GenerateLowEnergyAlert();
                    }   
                }
            }
            
            _previousResources = newData;
        }

        private void GenerateLowEnergyAlert()
        {
            var message = $"Colonists are freezing to death on {_planet.planetName}!";
            Notifications.Get().Send(new PlanetNotification {location = _planet, message = message});
        }
    }
}