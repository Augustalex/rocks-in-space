using System;
using UnityEngine;

namespace Interactors
{
    [Serializable]
    public struct InteractorCostsData
    {
        public int cash;
        public int ore;
        public int metals;
        public int gadgets;
    }

    public abstract class InteractorModule : MonoBehaviour
    {
        public InteractorCostsData costs;

        public virtual string GetCannotBuildHereMessage(Block block)
        {
            if (!block.GetConnectedPlanet().HasPort())
            {
                return "Asteroid needs a port before anything can be done here!";
            }

            if (!HasEnoughResourceToBuild(block))
            {
                var (neededResource, costAmount) = CheckMostUrgentResourceRequirement(block);
                return
                    $"Not enough {TinyPlanetResources.ResourceName(neededResource)} need {costAmount}!";
            }

            return "Can't build here!";
        }

        public virtual bool CanBuild(Block block)
        {
            return block.GetConnectedPlanet().HasPort() && HasEnoughResourceToBuild(block) && block.CanSeed();
        }

        protected bool HasEnoughResourceToBuild(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            if (resources.GetOre() < costs.ore) return false;
            if (resources.GetMetals() < costs.metals) return false;
            if (resources.GetGadgets() < costs.gadgets) return false;
            return true;
        }

        protected Tuple<TinyPlanetResources.PlanetResourceType, int> CheckMostUrgentResourceRequirement(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            if (resources.GetOre() < costs.ore)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.Ore, costs.ore);
            if (resources.GetMetals() < costs.metals)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.Metals, costs.metals);
            if (resources.GetGadgets() < costs.gadgets)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.Gadgets, costs.gadgets);
            throw new Exception("There are no requirement needs");
        }

        protected void ConsumeRequiredResources(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            resources.RemoveOre(costs.ore);
            resources.RemoveMetals(costs.metals);
            resources.RemoveGadgets(costs.gadgets);
        }

        protected void SetSeedRefund(GameObject seed)
        {
            var resourceSpent = seed.AddComponent<ResourceSpent>();
            resourceSpent.costs = costs;
        }

        public abstract float MaxActivationDistance();

        public abstract string GetInteractorName();

        public abstract string GetInteractorShortDescription();

        public abstract void Build(Block block, RaycastHit raycastHit);

        public abstract void OnFailedToBuild(Vector3 hitPoint);

        public abstract void OnBuilt(Vector3 hitPoint);

        public abstract void OnSecondaryInteract(Block block, RaycastHit hit);

        public abstract bool Continuous();

        public abstract bool Hoverable();

        public abstract void Hover(RaycastHit hit);
    }
}