using UnityEngine;

namespace Interactors
{
    public class PortInteractor : InteractorModule
    {
        private const double PortCost = 800;
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Port";
        }

        public override string GetInteractorShortDescription()
        {
            return "Place port to colonise asteroid";
        }

        public override bool CanBuild(Block block)
        {
            return !AsteroidAlreadyHasPort(block) && AllowedMorePorts();
        }

        private static bool AsteroidAlreadyHasPort(Block block)
        {
            return block.GetConnectedPlanet().HasPort();
        }

        private bool AllowedMorePorts()
        {
            var cash = GlobalResources.Get().GetCash();
            return cash > PortCost;
        }

        public override void Build(Block block)
        {
            var port = block.Seed(template);
            var portController = port.GetComponentInChildren<PortController>();
            var connectedPlanet = block.GetConnectedPlanet();
            connectedPlanet.AttachPort(portController);
            GlobalResources.Get().UseCash(800);

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
            
            InteractorController.Get().SetInteractorByName(DigInteractor.DigInteractorName); // It's unlikely the player want's to Port, digging is the most likely used next tool.
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
                    $"This asteroid already has a port";
            }
            else if (!AllowedMorePorts())
            {
                return "Need more inhabitants to build more ports";
            }

            return "Can't build here";
        }
    }
}