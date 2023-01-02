using System;
using UnityEngine;

namespace Interactors
{
    public class SelectInteractor : InteractorModule
    {
        public static string SelectInteractorName = "Select";
        
        [NonSerialized]
        private GameObject _lastCenteredPlanet;

        private static SelectInteractor _instance;
        private CurrentPlanetController _currentPlanetController;

        public event Action<RaycastHit> OnHover;

        void Start()
        {
            _currentPlanetController = CurrentPlanetController.Get();
        }

        public static SelectInteractor Get()
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SelectInteractor>();
            }

            return _instance;
        }
        
        public override string GetInteractorName()
        {
            return SelectInteractorName;
        }

        public override string GetInteractorShortDescription()
        {
            return $"Click on another asteroid to go there";
        }

        public override bool CanBuild(Block block)
        {
            var cameraController = CameraController.Get();
            var blocksPlanet = block.GetConnectedPlanet().gameObject;

            if (_lastCenteredPlanet != blocksPlanet)
            {
                return cameraController.AvailableToUpdate() 
                    && !block.IsSeeded(); // TODO: Why do we need to check if it is seeded or not? What does that have to do with being able to go there or not?
            }
            else
            {
                return block.GetRoot().GetComponentInChildren<Selectable.Selectable>();
            } 
        }

        public override void Build(Block block, RaycastHit raycastHit)
        {
            var cameraController = CameraController.Get();
            var blocksPlanet = block.GetConnectedPlanet().gameObject;

            if (_lastCenteredPlanet != blocksPlanet)
            {
                var planetBlock = block.GetConnectedPlanet().gameObject;
                var planet = planetBlock.GetComponent<TinyPlanet>();
                _currentPlanetController.ChangePlanet(planet);
                _lastCenteredPlanet = planet.gameObject;
                
                cameraController.FocusOnPlanet(planet);
            }
            else
            {
                block.GetRoot().GetComponentInChildren<Selectable.Selectable>().Select();
            }
        }

        public override void OnFailedToBuild(Vector3 hitPoint)
        {
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
            
        }
        
        public override void OnSecondaryInteract(Block block, RaycastHit hit)
        {
            
        }

        public override bool Continuous()
        {
            return false;
        }

        public override bool Hoverable()
        {
            return true;
        }

        public override void Hover(RaycastHit hit)
        {
            OnHover?.Invoke(hit);
        }

        public override string GetCannotBuildHereMessage(Block block)
        {
            // Not called
            return "";
        }

        public override float MaxActivationDistance()
        {
            return 2500f;
        }

        public void ForceSetLastConnectedPlanet(TinyPlanet startingPlanet)
        {
            _lastCenteredPlanet = startingPlanet.gameObject;
        }
    }
}