using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interactors
{
    [Serializable]
    public struct InteractorCostsData
    {
        public int cash;
        public int ore;
        [FormerlySerializedAs("metals")] public int ironPlates;
        public int copperPlates;
        public int gadgets;
    }

    [Serializable]
    public enum InteractorType
    {
        Dig,
        Port,
        Refinery,
        Factory,
        PowerPlant,
        FarmDome,
        ResidentModule,
        Platform,
        KorvKiosk,
        GeneralBuilding,
        Select,
        Misc
    }

    [Serializable]
    public enum BuildingType
    {
        Port = 0,
        Refinery = 1,
        CopperRefinery = 13,
        Factory = 2,
        PowerPlant = 3,
        FarmDome = 4,
        ResidentModule = 5,
        Lander = 6,
        Platform = 7,
        KorvKiosk = 8,
        Purifier = 9,
        Distillery = 10,
        SolarPanels = 11,
        ProteinFabricator = 12
    }

    public abstract class InteractorModule : MonoBehaviour
    {
        public GameObject
            template; // TODO: Could there be a BuildInteractor subclass that defines this attribute instead? Since it is not used by action interactors.

        public InteractorCostsData costs;

        public virtual InteractorType GetInteractorType()
        {
            return InteractorType.Misc;
        }

        public virtual bool GetIsAdditive()
        {
            // False=Replaces target block, True=Adds new block onto that block
            return false;
        }

        public virtual string GetCannotBuildHereMessage(Block block)
        {
            if (!block.GetConnectedPlanet().HasPort())
            {
                return "Asteroid needs a Beacon before anything can be done here!";
            }

            if (!GetIsAdditive() && !block.CanSeed())
            {
                return "Can't build here!";
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
            return block.GetConnectedPlanet().HasPort()
                   && HasEnoughResourceToBuild(block)
                   && block.CanSeed()
                   && block.GetConnectedPlanet().PlanetId.Is(CurrentPlanetController.Get().CurrentPlanet().PlanetId);
        }

        protected bool HasEnoughResourceToBuild(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            if (GlobalResources.Get().GetCash() < costs.cash) return false;
            if (resources.GetOre() < costs.ore) return false;
            if (resources.GetIronPlates() < costs.ironPlates) return false;
            if (resources.GetGadgets() < costs.gadgets) return false;
            return true;
        }

        protected Tuple<TinyPlanetResources.PlanetResourceType, int> CheckMostUrgentResourceRequirement(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            if (GlobalResources.Get().GetCash() < costs.cash)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.Cash, costs.cash);
            if (resources.GetOre() < costs.ore)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.Ore, costs.ore);
            if (resources.GetIronPlates() < costs.ironPlates)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.IronPlates, costs.ironPlates);
            if (resources.GetResource(TinyPlanetResources.PlanetResourceType.CopperPlates) < costs.copperPlates)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.CopperPlates, costs.copperPlates);
            if (resources.GetGadgets() < costs.gadgets)
                return new Tuple<TinyPlanetResources.PlanetResourceType, int>(
                    TinyPlanetResources.PlanetResourceType.Gadgets, costs.gadgets);
            throw new Exception("There are no requirement needs");
        }

        protected void ConsumeRequiredResources(Block block)
        {
            var resources = block.GetConnectedPlanet().GetResources();
            GlobalResources.Get().UseCash(costs.cash);
            resources.RemoveOre(costs.ore);
            resources.RemoveIronPlates(costs.ironPlates);
            resources.RemoveGadgets(costs.gadgets);
        }

        protected void SetSeedRefund(GameObject seed)
        {
            var resourceSpent = seed.AddComponent<ResourceSpent>();
            resourceSpent.costs = costs;
        }

        public abstract string GetInteractorName();

        public abstract string GetInteractorShortDescription();

        public abstract void Build(Block block, RaycastHit raycastHit);

        public virtual void OnFailedToBuild(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();

            audioController.Play(audioController.cannotBuild, audioController.cannotBuildVolume,
                hitPoint);
        }
        
        public virtual void OnBuilt(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();

            audioController.Play(audioController.build, audioController.buildVolume,
                hitPoint);
        }

        public virtual void OnSecondaryInteract(Block block, RaycastHit hit)
        {
            // Do nothing
        }

        public virtual bool Continuous()
        {
            return false;
        }

        public virtual float MaxActivationDistance()
        {
            return 60f;
        }

        public virtual bool Hoverable()
        {
            return false;
        }

        public virtual void Hover(RaycastHit hit)
        {
            // Do nothing
        }
    }
}