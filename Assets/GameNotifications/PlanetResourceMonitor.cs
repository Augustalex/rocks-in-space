using System;
using System.Collections;
using UnityEngine;

namespace GameNotifications
{
    public class PlanetResourceMonitor : MonoBehaviour
    {
        private bool _hasHadCopper;
        private bool _hasSentCopperHint;

        private TinyPlanet _planet;
        private TinyPlanetResources _resources;
        private TinyPlanetResources.ResourcesData _previousResources;
        private readonly NotificationThrottler _outOfIron = new();
        private readonly NotificationThrottler _noIceNotification = new();
        private readonly NotificationThrottler _freezingColonistsNotification = new();
        private readonly NotificationThrottler _lowEnergyNotification = new();
        private readonly NotificationThrottler _lowFoodNotification = new();
        private readonly NotificationThrottler _outOfGraphite = new();
        private readonly NotificationThrottler _outOfCopper = new();
        private readonly TwoWayNotificationThrottler _newColonists = new(10);

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
            var currentAmountOfInhabitants = newData.Inhabitants - newData.Landers;
            var previousAmountOfInhabitants = _previousResources.Inhabitants - _previousResources.Landers;
            var landerSettledThisFrame = _previousResources.Landers < newData.Landers;

            if (Math.Abs(newData.Energy - _previousResources.Energy) > .5f)
            {
                if (noEnergy)
                {
                    if (currentAmountOfInhabitants > 0)
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
                if (currentAmountOfInhabitants > 0)
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
                    _outOfIron.SendIfCanPost(
                        CreatePlanetNotification(
                            $"{_planet.planetName} has run out of {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.IronOre)}!"
                        )
                    );
                }
            }

            if (Math.Abs(newData.Graphite - _previousResources.Graphite) > .5f)
            {
                if (newData.Graphite <= 0.5f)
                {
                    _outOfGraphite.SendIfCanPost(
                        CreatePlanetNotification(
                            $"{_planet.planetName} has run out of {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Graphite)}!"
                        )
                    );
                }
            }

            if (Math.Abs(newData.Copper - _previousResources.Copper) > .5f)
            {
                if (newData.Copper <= 0.5f)
                {
                    _outOfCopper.SendIfCanPost(
                        CreatePlanetNotification(
                            $"{_planet.planetName} has run out of {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.CopperOre)}!"
                        )
                    );
                }
            }

            if (Math.Abs(newData.Ice - _previousResources.Ice) > .5f)
            {
                if (newData.Ice <= .5f)
                {
                    _noIceNotification.SendIfCanPost(
                        CreatePlanetNotification(
                            $"Purifiers have no {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Ice)} to work with on {_planet.planetName}!"
                        )
                    );
                }
            }

            var amountOfInhabitantsChanged = Math.Abs(currentAmountOfInhabitants - previousAmountOfInhabitants);
            if (!landerSettledThisFrame && amountOfInhabitantsChanged > .5f)
            {
                if (newData.Inhabitants > _previousResources.Inhabitants)
                {
                    _newColonists.SendIfCanPost(
                        CreatePlanetNotification(
                            $"Colonists are moving into {_planet.planetName}"
                        ),
                        true
                    );
                }
                else
                {
                    _newColonists.SendIfCanPost(
                        CreatePlanetNotification(
                            $"Colonists are leaving {_planet.planetName}"
                        ),
                        false
                    );
                }
                
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

        private PlanetNotification CreatePlanetNotification(string message,
            NotificationTypes notificationType = NotificationTypes.Alerting)
        {
            return new PlanetNotification
            {
                Location = _planet,
                NotificationType = notificationType,
                Message = message
            };
        }

        private void GenerateLowEnergyAlert()
        {
            _lowEnergyNotification.SendIfCanPost(
                CreatePlanetNotification(
                    $"Not enough {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)} on {_planet.planetName}!"
                )
            );
        }

        private void GenerateFreezingColonistsAlert()
        {
            _freezingColonistsNotification.SendIfCanPost(
                CreatePlanetNotification(
                    $"No {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Energy)} as colonists are freezing to death on {_planet.planetName}!"
                )
            );
        }

        private void GenerateLowProteinAlert()
        {
            _lowFoodNotification.SendIfCanPost(
                CreatePlanetNotification(
                    $"Starvation rampant on {_planet.planetName}!"
                )
            );
        }
    }
}