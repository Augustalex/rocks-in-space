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
        private PlanetNotification _outOfIron;
        private PlanetNotification _noIceNotification;
        private PlanetNotification _freezingColonistsNotification;
        private PlanetNotification _lowEnergyNotification;
        private PlanetNotification _lowFoodNotification;
        private PlanetNotification _outOfGraphite;
        private PlanetNotification _outOfCopper;

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
                yield return new WaitForSeconds(1f);
                Check();
            }
        }

        private void Check()
        {
            var newData = _resources.CopyData();
            var noEnergy = newData.Energy < 0f;

            if (Math.Abs(newData.Energy - _previousResources.Energy) > .5f)
            {
                if (noEnergy)
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

            if (Math.Abs(newData.Protein - _previousResources.Protein) > .5f)
            {
                if (newData.Inhabitants > 0)
                {
                    if (newData.Protein <= 0f)
                    {
                        GenerateLowProteinAlert();
                    }
                }
            }

            if (Math.Abs(newData.Iron - _previousResources.Iron) > .5f)
            {
                if (newData.Iron <= 0.5f)
                {
                    GenerateNoMoreIronAlert();
                }
            }

            if (Math.Abs(newData.Graphite - _previousResources.Graphite) > .5f)
            {
                if (newData.Graphite <= 0.5f)
                {
                    GenerateNoMoreGraphiteAlert();
                }
            }

            if (Math.Abs(newData.Copper - _previousResources.Copper) > .5f)
            {
                if (newData.Copper <= 0.5f)
                {
                    GenerateNoMoreCopperAlert();
                }
            }

            if (Math.Abs(newData.Ice - _previousResources.Ice) > .5f)
            {
                if (newData.Ice <= .5f)
                {
                    GenerateNoMoreIceAlert();
                }
            }

            if (Math.Abs(newData.Inhabitants - _previousResources.Inhabitants) > .5f)
            {
                if (noEnergy)
                {
                    GenerateFreezingColonistsAlert();
                }
                else if (newData.Protein <= 0f)
                {
                    GenerateLowProteinAlert();
                }
            }

            _previousResources = newData;
        }

        private void GenerateNoMoreIronAlert()
        {
            if (_outOfIron != null && !_outOfIron.Closed()) return;

            var message =
                $"No more {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Iron)} to work with on {_planet.planetName}!";
            var notification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Alerting };
            _outOfIron = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateNoMoreGraphiteAlert()
        {
            if (_outOfGraphite != null && !_outOfGraphite.Closed()) return;

            var message =
                $"No more {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Graphite)} available at {_planet.planetName}!";
            var notification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Alerting };
            _outOfGraphite = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateNoMoreCopperAlert()
        {
            if (_outOfCopper != null && !_outOfCopper.Closed()) return;

            var message =
                $"Factories have no {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Copper)} to work with on {_planet.planetName}!";
            var notification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Alerting };
            _outOfCopper = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateNoMoreIceAlert()
        {
            if (_noIceNotification != null && !_noIceNotification.Closed()) return;

            var message =
                $"Purifiers have no {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Ice)} to work with on {_planet.planetName}!";
            var notification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Alerting };
            _noIceNotification = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateLowEnergyAlert()
        {
            if (_lowEnergyNotification != null && !_lowEnergyNotification.Closed()) return;

            var message =
                $"Not enough {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)} on {_planet.planetName}!";
            var lowEnergyNotification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Informative };
            _lowEnergyNotification = lowEnergyNotification;

            Notifications.Get().Send(lowEnergyNotification);
        }

        private void GenerateFreezingColonistsAlert()
        {
            if (_freezingColonistsNotification != null && !_freezingColonistsNotification.Closed()) return;

            var message =
                $"No {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)} as colonists are freezing to death on {_planet.planetName}!";
            var notification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Alerting };
            _freezingColonistsNotification = notification;

            Notifications.Get().Send(notification);
        }

        private void GenerateLowProteinAlert()
        {
            if (_lowFoodNotification != null && !_lowFoodNotification.Closed()) return;

            var message = $"Starvation rampant on {_planet.planetName}!";
            var lowFoodNotification = new PlanetNotification
                { Location = _planet, Message = message, NotificationType = NotificationTypes.Alerting };
            _lowFoodNotification = lowFoodNotification;

            Notifications.Get().Send(lowFoodNotification);
        }
    }
}