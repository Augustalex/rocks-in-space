using System;
using UnityEngine;

namespace Interactors
{
    public class FactoryInteractor : InteractorModule
    {
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Factory";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            return resources.GetMetals() >= 50;
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            resources.SetMetals(resources.GetMetals() - 50);
            
            block.Seed(template);
        }

        public override bool Continuous()
        {
            return false;
        }
    }
}