using System;
using UnityEngine;

namespace Interactors
{
    public class SelectInteractor : InteractorModule
    {
        [NonSerialized]
        private GameObject _lastCenteredPlanet;

        private static SelectInteractor _instance;

        public event Action<RaycastHit> OnHover;

        public static SelectInteractor Get()
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SelectInteractor>();
            }

            return _instance;
        }

        public GameObject GetLastCenteredPlanet()
        {
            return _lastCenteredPlanet;
        }
        
        public override string GetInteractorName()
        {
            return "Select";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            var cameraController = CameraController.Get();
            var blocksPlanet = block.GetConnectedPlanet().gameObject;
            
            return cameraController.AvailableToUpdate() 
                   && _lastCenteredPlanet != blocksPlanet
                   && !block.IsSeeded();
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            _lastCenteredPlanet = block.GetConnectedPlanet().gameObject;
            
            CameraController.Get().FocusOnPlanet(block);
        }

        public override void OnFailedToBuild(Vector3 hitPoint)
        {
        }

        public override void OnBuilt(Vector3 hitPoint)
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
    }
}