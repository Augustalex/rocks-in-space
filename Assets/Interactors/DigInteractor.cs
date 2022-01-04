using System;
using UnityEngine;

namespace Interactors
{
    public class DigInteractor : InteractorModule
    {
        public GameObject laserLight;
        public LaserEffect laserEffect;

        private const float Cooldown = .14f;

        [NonSerialized] private float _lastDig = 1f;

        private bool _started;
        private float _startedAt;
        private float _actionLength = 3f;
        private Block _activeTargetBlock;
        private Camera _camera;
        private Vector3 _targetPosition;

        private void Start()
        {
            _camera = Camera.main;
        }

        public override string GetInteractorName()
        {
            return "Dig";
        }

        public bool Started()
        {
            return _started;
        }

        public void StartInteraction(Block block)
        {
            _started = true;
            _startedAt = Time.time;

            _activeTargetBlock = block;
        }

        public void StopInteraction()
        {
            _started = false;

            if (_activeTargetBlock)
            {
                _activeTargetBlock.GetMesh().ResetHeat();
                _activeTargetBlock = null;
            }
        }

        public bool FinishedInteraction()
        {
            if (!_started) return false;

            return Time.time - _startedAt > _actionLength;
        }

        public void FinishInteraction()
        {
            _activeTargetBlock.Dig();

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

                if (!laserEffect.IsActivated())
                {
                    laserEffect.Activate();
                }

                laserEffect.SetTarget(_targetPosition);
                if (_activeTargetBlock)
                {
                    var interactionDuration = Time.time - _startedAt;
                    var completionFactor = interactionDuration / _actionLength;
                    _activeTargetBlock.GetMesh().SetHeat(completionFactor);
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
                var block = hit.collider.GetComponent<Block>();
                if (block != _activeTargetBlock)
                {
                    StopInteraction();
                }

                _targetPosition = hit.point;
            }
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            var time = Time.time;
            var timeSinceLastBuilt = time - _lastDig;
            return timeSinceLastBuilt > Cooldown && !block.IsSeeded();
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            // var time = Time.time;
            // _lastDig = time;

            // block.Dig();
        }

        public override void OnFailedToBuild(Vector3 hitPoint)
        {
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();

            audioController.Play(audioController.destroyBlock, audioController.destroyBlockVolume,
                hitPoint);
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
        }
    }
}