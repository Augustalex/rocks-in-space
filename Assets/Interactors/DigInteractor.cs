﻿using System;
using UnityEngine;

namespace Interactors
{
    public class DigInteractor : InteractorModule
    {
        private const float Cooldown = .14f;
        private float _lastBuilt;

        public override string GetInteractorName()
        {
            return "Dig";
        }

        public override bool CanBuild(Block block, TinyPlanetResources resources)
        {
            var timeSinceLastBuilt = Time.time - _lastBuilt;
            return timeSinceLastBuilt > Cooldown && !block.IsSeeded();
        }

        public override void Build(Block block, TinyPlanetResources resources)
        {
            _lastBuilt = Time.time;
            
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