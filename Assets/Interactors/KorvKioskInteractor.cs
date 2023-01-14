using UnityEngine;

namespace Interactors
{
    public class KorvKioskInteractor : InteractorModule
    {
        public override InteractorType GetInteractorType()
        {
            return InteractorType.KorvKiosk;
        }

        public static string GetName()
        {
            return "Mackapar";
        }

        public override string GetInteractorName()
        {
            return GetName();
        }

        public override string GetInteractorShortDescription()
        {
            return $"Placing {GetInteractorName()}";
        }

        public override bool CanBuild(Block block)
        {
            return block.GetConnectedPlanet().HasPort()
                   && HasEnoughResourceToBuild(block);
        }

        public override void Build(Block block, RaycastHit raycastHit)
        {
            ConsumeRequiredResources(block);
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