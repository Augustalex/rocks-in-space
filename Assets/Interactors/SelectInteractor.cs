using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Interactors
{
    public class SelectInteractor : InteractorModule
    {
        [NonSerialized]
        private GameObject _lastCenteredPlanet;

        private static SelectInteractor _instance;
        private CurrentPlanetController _currentPlanetController;

        public event Action<RaycastHit> OnHover;
        public event Action<RaycastHit> OnClick;

        public event Action<RaycastHit> OnContext;

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
            return "Select";
        }

        public override bool CanBuild(Block block)
        {
            var cameraController = CameraController.Get();
            var blocksPlanet = block.GetConnectedPlanet().gameObject;

            if (_lastCenteredPlanet != blocksPlanet)
            {
                return cameraController.AvailableToUpdate() 
                    && !block.IsSeeded();
            }
            else
            {
                return block.GetRoot().GetComponentInChildren<Selectable.Selectable>();
            } 
        }

        public override void Build(Block block, TinyPlanetResources resources)
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

        public override float MaxActivationDistance()
        {
            return 1000f;
        }

        public void ForceSetLastConnectedPlanet(TinyPlanet startingPlanet)
        {
            _lastCenteredPlanet = startingPlanet.gameObject;
        }
    }
}