using System;
using UnityEngine;

namespace Interactors
{
    public class DigInteractor : InteractorModule
    {
        private const float Cooldown = .14f;

        [NonSerialized]
        private float _lastDig = 1f;

        public override string GetInteractorName()
        {
            return "Dig";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            var time = Time.time;
            var timeSinceLastBuilt = time - _lastDig;
            return timeSinceLastBuilt > Cooldown && !block.IsSeeded();
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            var time = Time.time;
            _lastDig = time;
            
            block.Dig();
        }
        
        public override void OnFailedToBuild(Vector3 hitPoint)
        {
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
            var audioController = AudioController.Get();
            
            audioController.Play(audioController.destroyBlock, audioController.destroyBlockVolume,
                hitPoint);
        }

        public override bool Continuous()
        {
            return true;
        }
        
        public override float MaxActivationDistance()
        {
            return 60f;
        }
    }
}