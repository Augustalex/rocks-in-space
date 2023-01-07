using UnityEngine;

namespace Interactors
{
    public class PortInteractor : InteractorModule
    {
        public override InteractorType GetInteractorType()
        {
            return InteractorType.Port;
        }

        public override string GetInteractorName()
        {
            return "Port";
        }

        public override string GetInteractorShortDescription()
        {
            return "Placing Beacon";
        }

        public override bool CanBuild(Block block)
        {
            return !AsteroidAlreadyHasPort(block) && HasEnoughResourceToBuild(block);
        }

        private static bool AsteroidAlreadyHasPort(Block block)
        {
            return block.GetConnectedPlanet().HasPort();
        }

        public override void Build(Block block, RaycastHit raycastHit)
        {
            ConsumeRequiredResources(block);

            var port = block.Seed(template);
            var portController = port.GetComponentInChildren<PortController>();
            var connectedPlanet = block.GetConnectedPlanet();
            connectedPlanet.AttachPort(portController);

            var displayController = DisplayController.Get();
            if (displayController.PlanetInFocus(connectedPlanet))
            {
                displayController.StartRenamingPlanet();
            }
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

            InteractorController.Get()
                .SetInteractorByName(DigInteractor
                    .DigInteractorName); // It's unlikely the player want's to Port, digging is the most likely used next tool.
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

        public override string GetCannotBuildHereMessage(Block block)
        {
            if (AsteroidAlreadyHasPort(block))
            {
                return
                    $"This asteroid already has a Beacon";
            }
            else if (!HasEnoughResourceToBuild(block))
            {
                return "Not enough credits to build another Beacon";
            }

            return "Can't build here";
        }
    }
}