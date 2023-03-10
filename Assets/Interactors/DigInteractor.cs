using System;
using Interactors.Digging;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interactors
{
    public class DigInteractor : InteractorModule
    {
        public const string DigInteractorName = "Dig";

        public GameObject laserLight;
        public LaserEffect laserEffect;

        public event Action<RaycastHit> OnHover;

        private const float
            Cooldown = .14f; // There needs to be a cooldown - since a block exists for 1 frame between when it has been asked to be destroyed, and until it has actually been destroyed. 

        [NonSerialized] private float _lastDig = 1f;

        private bool _started;
        private float _startedAt;
        private ILaserInteractable _activeTargetEntity;
        private Camera _camera;
        private Vector3 _targetPosition;
        private Vector3 _previousMousePosition;
        private float _lastInteractionAudio;

        private readonly string[] _texts =
        {
            "It's not a good idea to laser your Beacon, leave it for now.",
            "It's really not smart to laser your Beacon. Bad things could happen!",
            "Okay, nothing bad would happen. It would just be messy, alright?",
            "Imagine this. You're having a great day, mining resources, and OOPS you have lasered your Beacon!",
            "Now without a Beacon, you wouldn't be able to dig or build anything! Not good.",
            "AND what if you couldn't afford another one? Then what. It would all be over wouldn't it?",
            "All for making the silly mistake of lasering your own Beacon.",
            "You can't laser your own Beacon, sorry.",
        };

        private int _textIndex = 0;
        private float _lastChangedIndex;
        private float _noFailMessageUntil;

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
            return "";
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
            AudioController.Get().Cancel();

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
                        var audioController = AudioController.Get();
                        audioController.PlayWithRandomPitch(audioController.laserStarted,
                            audioController.laserStartedVolume, .15f);
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

                            if (Time.time - _lastInteractionAudio > .15f && Random.value < .05)
                            {
                                // var audioController = AudioController.Get();
                                // audioController.Play(audioController.laserProgress, audioController.laserProgressVolume,
                                //     _activeTargetEntity.GetAudioPosition());

                                _lastInteractionAudio = Time.time;
                            }
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
        }

        public void FinishInteraction()
        {
            _activeTargetEntity.LaserInteract();

            StopInteraction();
        }

        private void UpdateCursorPosition()
        {
            var mousePosition = Input.mousePosition;
            var ray = _camera.ScreenPointToRay(mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, MaxActivationDistance()))
            {
                var laserableEntity = hit.collider.GetComponent<ILaserInteractable>();
                if (_started && laserableEntity != _activeTargetEntity)
                {
                    var distanceChange = Vector2.Distance(mousePosition, _previousMousePosition);
                    if (distanceChange > 2f)
                    {
                        StopInteraction();
                    }
                }
                else
                {
                    _targetPosition = hit.point;
                    laserLight.transform.LookAt(_targetPosition);
                }
            }

            _previousMousePosition = mousePosition;
        }

        public override bool CanBuild(Block block)
        {
            return false;
        }

        public bool CanPerformInteraction(ILaserInteractable laserableEntity)
        {
            return laserableEntity.CanInteract();
        }

        public bool IsCooledDown()
        {
            var time = Time.time;
            var timeSinceLastBuilt = time - _lastDig;
            return timeSinceLastBuilt > Cooldown;
        }

        public override void Build(Block block, RaycastHit raycastHit)
        {
            // Do nothing
        }

        public override void OnFailedToBuild(Vector3 hitPoint)
        {
            // Do nothing
        }

        public override string GetCannotBuildHereMessage(Block block)
        {
            if (!block.GetConnectedPlanet().HasPort())
            {
                // return "Asteroid needs a Beacon before anything can be done here!";
                return "Build a Beacon before digging here";
            }

            if (block.IsSeeded() && block.GetRoot().GetComponentInChildren<PortController>())
            {
                if (_lastChangedIndex > 0f && Time.time - _lastChangedIndex > 2f)
                {
                    _textIndex += 1;
                }

                if (_textIndex >= _texts.Length)
                {
                    _lastChangedIndex = Time.time;

                    if (Random.value < .5f) return "Nope.";
                    if (Random.value < .5f) return "Sorry!";
                    if (Random.value < .5f) return "Can't do.";
                    if (Random.value < .5f) return "Give it up.";
                    if (Random.value < .5f) return "Maybe if you try one more time it would work?";
                    if (Random.value < .5f) return "It's never going to work.";
                    if (Random.value < .5f) return "Stop trying, you are wasting time.";
                    return "Maybe take a walk? You seem tired, not thinking straight.";
                }
                else
                {
                    var text = _texts[_textIndex];
                    _lastChangedIndex = Time.time;

                    return text;
                }
            }

            return "Can't use your laser here.";
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();
            audioController.Play(audioController.build, audioController.buildVolume,
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
            return 999f;
        }

        public override bool Hoverable()
        {
            return true;
        }

        public override void Hover(RaycastHit hit)
        {
            OnHover?.Invoke(hit);
        }

        public void Navigate(TinyPlanet planet)
        {
            _noFailMessageUntil = Time.time + 1f;
            var cameraController = CameraController.Get();
            CurrentPlanetController.Get().ChangePlanet(planet);
            cameraController.FocusOnPlanet(planet);
        }

        public bool CanShowFailMessage()
        {
            return Time.time > _noFailMessageUntil;
        }
    }
}