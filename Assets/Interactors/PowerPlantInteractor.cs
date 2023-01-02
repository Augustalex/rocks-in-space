using System;
using UnityEngine;

namespace Interactors
{
    public class PowerPlantInteractor : InteractorModule
    {
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Power plant";
        }

        public override string GetInteractorShortDescription()
        {
            return $"Place {GetInteractorName()}";
        }
        
        public override void Build(Block block, RaycastHit raycastHit)
        {
            ConsumeRequiredResources(block);

            var seed = block.Seed(template);
            SetSeedRefund(seed);
        }
        
        public override void OnFailedToBuild(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();

            audioController.Play(audioController.cannotBuild, audioController.cannotBuildVolume,
                hitPoint);
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();
            
            audioController.Play(audioController.destroyBlock, audioController.destroyBlockVolume,
                hitPoint);
        }

        public override void OnSecondaryInteract(Block block, RaycastHit hit)
        {
            // Do nothing
        }

        public override bool Continuous()
        {
            return false;
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