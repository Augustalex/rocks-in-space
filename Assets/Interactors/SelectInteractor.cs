using System;
using UnityEngine;

namespace Interactors
{
    public class SelectInteractor : InteractorModule
    {
        private GameObject _lastCenteredPlanet;
        
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

        public override float MaxActivationDistance()
        {
            return 1000f;
        }
    }
}