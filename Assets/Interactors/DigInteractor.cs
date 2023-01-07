using System;
using Interactors.Digging;
using UnityEngine;

namespace Interactors
{
    public class DigInteractor : InteractorModule
    {
        public const string DigInteractorName = "Dig";

        public GameObject laserLight;
        public LaserEffect laserEffect;

        private const float Cooldown = .14f;

        [NonSerialized] private float _lastDig = 1f;

        private bool _started;
        private float _startedAt;
        private ILaserInteractable _activeTargetEntity;
        private Camera _camera;
        private Vector3 _targetPosition;

        private void Start()
        {
            _camera = GetComponentInParent<Camera>();
        }

        public override InteractorType GetInteractorType()
        {
            return InteractorType.Dig;
        }

        public override string GetInteractorName()
        {
            return DigInteractorName;
        }

        public override string GetInteractorShortDescription()
        {
            return "Laser active";
        }

        public bool Started()
        {
            return _started && _activeTargetEntity != null;
        }

        public void StartInteraction(ILaserInteractable targetEntity)
        {
            _started = true;
            _startedAt = Time.time;

            _activeTargetEntity = targetEntity;
        }

        public void StopInteraction()
        {
            _started = false;
            _lastDig = Time.time;

            try
            {
                if (_activeTargetEntity != null && _activeTargetEntity.CanInteract())
                {
                    _activeTargetEntity.GetOven().ResetHeat();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                _activeTargetEntity = null;
            }
        }

        public bool FinishedInteraction()
        {
            return !_started || (_started && Time.time - _startedAt > ActionLength());
        }

        private float ActionLength()
        {
            return _activeTargetEntity.DisintegrationTime();
        }

        public void FinishInteraction()
        {
            _activeTargetEntity.LaserInteract();
            StopInteraction();
        }

        private void Update()
        {
            if (_started)
            {
                if (FinishedInteraction())
                {
                    FinishInteraction();
                }
                else
                {
                    if (!laserEffect.IsActivated())
                    {
                        laserEffect.Activate();
                    }

                    laserEffect.SetTarget(_targetPosition);
                    if (_activeTargetEntity != null)
                    {
                        if (_activeTargetEntity.CanInteract())
                        {
                            var interactionDuration = Time.time - _startedAt;
                            var completionFactor = interactionDuration / ActionLength();
                            _activeTargetEntity.GetOven().SetHeat(completionFactor);
                        }
                    }
                }
            }
            else
            {
                if (laserEffect.IsActivated())
                {
                    laserEffect.Stop();
                }
            }

            UpdateCursorPosition();
            laserLight.transform.LookAt(_targetPosition);
        }

        private void UpdateCursorPosition()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, MaxActivationDistance()))
            {
                var laserableEntity = hit.collider.GetComponent<ILaserInteractable>();
                if (_started && laserableEntity != _activeTargetEntity)
                {
                    StopInteraction();
                }

                _targetPosition = hit.point;
            }
        }

        public override bool CanBuild(Block block)
        {
            return false;
        }

        public bool CanPerformInteraction(ILaserInteractable laserableEntity)
        {
            var time = Time.time;
            var timeSinceLastBuilt = time - _lastDig;
            return timeSinceLastBuilt > Cooldown && laserableEntity.CanInteract();
        }

        public override void Build(Block block, RaycastHit raycastHit)
        {
            // Do nothing
        }

        public override void OnFailedToBuild(Vector3 hitPoint)
        {
            // Do nothing
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();
            audioController.Play(audioController.destroyBlock, audioController.destroyBlockVolume,
                hitPoint);
        }

        public override void OnSecondaryInteract(Block block, RaycastHit hit)
        {
            var newPosition = hit.normal * block.transform.localScale.x + block.transform.position;
            TinyPlanetGenerator.Get().CreateRockAndAttachToNearPlanet(newPosition);
        }

        public override bool Continuous()
        {
            return true;
        }

        public override float MaxActivationDistance()
        {
            return 60f;
        }

        public override bool Hoverable()
        {
            return false;
        }

        public override void Hover(RaycastHit hit)
        {
            // Do nothing
        }
    }
}