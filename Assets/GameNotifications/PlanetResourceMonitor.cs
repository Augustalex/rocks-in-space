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
        private PlanetNotification _outOfOreNotification;
        private PlanetNotification _freezingColonistsNotification;
        private PlanetNotification _lowEnergyNotification;
        private PlanetNotification _lowFoodNotification;

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
                if (newData.Energy <= 0f)
                {
                    if (newData.Inhabitants > 0)
                    {
                        GenerateFreezingColonistsAlert();
                    }
                    else
                    {
                        GenerateLowEnergyAlert();
                    }
                }
            }

            if (Math.Abs(newData.Food - _previousResources.Food) > .5f)
            {
                if (newData.Inhabitants > 0)
                {
                    if (newData.Food <= 10f)
                    {
                        GenerateLowFoodAlert();
                    }
                }
            }

            if (Math.Abs(newData.Ore - _previousResources.Ore) > .5f)
            {
                if (newData.Ore <= 0f)
                {
                    GenerateNoMoreOreAlert();
                }
            }

            _previousResources = newData;
        }

        private void GenerateNoMoreOreAlert()
        {
            if (_outOfOreNotification != null && !_outOfOreNotification.Closed()) return;

            var message = $"Refineries have nothing to work with on {_planet.planetName}!";
            var notification = new PlanetNotification { location = _planet, message = message };
            _outOfOreNotification = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateLowEnergyAlert()
        {
            if (_lowEnergyNotification != null && !_lowEnergyNotification.Closed()) return;

            var message = $"Not enough power on {_planet.planetName}!";
            var lowEnergyNotification = new PlanetNotification { location = _planet, message = message };
            _lowEnergyNotification = lowEnergyNotification;

            Notifications.Get().Send(lowEnergyNotification);
        }

        private void GenerateFreezingColonistsAlert()
        {
            if (_freezingColonistsNotification != null && !_freezingColonistsNotification.Closed()) return;

            var message = $"No power as colonists are freezing to death on {_planet.planetName}!";
            var notification = new PlanetNotification { location = _planet, message = message };
            _freezingColonistsNotification = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateLowFoodAlert()
        {
            if (_lowFoodNotification != null && !_lowFoodNotification.Closed()) return;

            var message = $"Starvation rampant on {_planet.planetName}!";
            var lowFoodNotification = new PlanetNotification { location = _planet, message = message };
            _lowFoodNotification = lowFoodNotification;

            Notifications.Get().Send(lowFoodNotification);
        }
    }
}