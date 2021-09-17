using System;
using UnityEngine;

namespace Interactors
{
    public class ResidencyInteractor : InteractorModule
    {
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Residency module";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            return resources.GetGadgets() >= 10;
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            resources.SetGadgets(resources.GetGadgets() - 10);
            
            block.Seed(template);
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



        public override bool Continuous()
        {
            return false;
        }
        
        public override float MaxActivationDistance()
        {
            return 60f;
        }
    }
}