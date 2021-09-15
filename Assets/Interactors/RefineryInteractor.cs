using System;
using UnityEngine;

namespace Interactors
{
    public class RefineryInteractor : InteractorModule
    {
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Ore refinery";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            return resources.GetOre() >= 500;
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            resources.SetOre(resources.GetOre() - 500);
            
            block.Seed(template);
        }
    }
}