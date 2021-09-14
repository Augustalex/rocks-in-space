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
            return true;
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            block.Seed(template);
        }
    }
}