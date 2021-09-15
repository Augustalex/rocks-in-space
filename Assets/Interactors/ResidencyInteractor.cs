using System;
using UnityEngine;

namespace Interactors
{
    public class ResidencyInteractor : InteractorModule
    {
        public GameObject template;

        public override string GetInteractorName()
        {
            return "Residency module";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            return resources.GetGadgets() >= 10;
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            resources.SetGadgets(resources.GetGadgets() - 10);
            
            block.Seed(template);
        }
    }
}