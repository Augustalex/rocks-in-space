using System;
using UnityEngine;

namespace Interactors
{
    public class PowerPlantInteractor : InteractorModule
    {
        private const int GadgetsCost = 100;
        
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Power plant";
        }

        public override string GetInteractorShortDescription()
        {
            return $"Place {GetInteractorName()}";
        }

        public override bool CanBuild(Block block)
        {
            return HasEnoughGadgets(block); 
        }

        private bool HasEnoughGadgets(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            return resources.GetGadgets() >= GadgetsCost;
        }

        public override void Build(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            resources.SetGadgets(resources.GetGadgets() - GadgetsCost);
            
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
            
        }

        public override string GetCannotBuildHereMessage(Block block)
        {
            if (!HasEnoughGadgets(block))
            {
                return
                    $"Not enough {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Gadgets)} need {GadgetsCost}";
            }

            return "Can't build here";
        }
    }
}