using UnityEngine;

namespace Interactors
{
    public class FarmDomeInteractor : InteractorModule
    {
        public override InteractorType GetInteractorType()
        {
            return InteractorType.FarmDome;
        }

        public static string GetName()
        {
            return "Farms";
        }
        
        public override string GetInteractorName()
        {
            return GetName();
        }

        public override string GetInteractorShortDescription()
        {
            return $"Placing {GetInteractorName()}";
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
            
            audioController.Play(audioController.build, audioController.buildVolume,
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
    }
}