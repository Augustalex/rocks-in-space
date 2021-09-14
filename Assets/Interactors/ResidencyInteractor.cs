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
            return resources.GetGadgets() >= 100;
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            resources.SetGadgets(resources.GetGadgets() - 100);
            
            block.Seed(template);
        }
    }
}