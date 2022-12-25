using UnityEngine;

namespace Interactors
{
    public class FactoryInteractor : InteractorModule
    {
        private const int MetalsCost = 50;
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Factory";
        }
        
        public override string GetInteractorShortDescription()
        {
            return $"Place {GetInteractorName()}";
        }
        
        public override bool CanBuild(Block block)
        {
            return HasEnoughMetals(block);
        }

        private static bool HasEnoughMetals(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            return resources.GetMetals() >= MetalsCost;
        }

        public override void Build(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            resources.SetMetals(resources.GetMetals() - MetalsCost);
            
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
            if (!HasEnoughMetals(block))
            {
                return
                    $"Not enough {TinyPlanetResources.ResourceName(TinyPlanetResources.PlanetResourceType.Metals)} need {MetalsCost}";
            }

            return "Can't build here";
        }
    }
}