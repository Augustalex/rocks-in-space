using System;
using UnityEngine;

namespace Interactors
{
    public class RefineryInteractor : InteractorModule
    {
        private const int OreCost = 500;
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Ore refinery";
        }
        
        public override string GetInteractorShortDescription()
        {
            return $"Place {GetInteractorName()}";
        }

        public override bool CanBuild(Block block)
        {
            return HasEnoughOre(block);
        }

        private static bool HasEnoughOre(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            return resources.GetOre() >= OreCost;
        }

        public override void Build(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            resources.SetOre(resources.GetOre() - OreCost);
            
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
            if (!HasEnoughOre(block))
            {
                return
                    $"Not enough {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Ore)} need {OreCost}";
            }

            return "Can't build here";
        }
    }
}