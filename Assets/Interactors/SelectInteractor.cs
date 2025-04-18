﻿using System;
using UnityEngine;

namespace Interactors
{
    public class SelectInteractor : InteractorModule
    {
        public static readonly string SelectInteractorName = "Select";

        private static SelectInteractor _instance;
        private CurrentPlanetController _currentPlanetController;

        public event Action<RaycastHit> OnHover;

        void Start()
        {
            _currentPlanetController = CurrentPlanetController.Get();
        }

        public override InteractorType GetInteractorType()
        {
            return InteractorType.Select;
        }

        public override string GetInteractorName()
        {
            return SelectInteractorName;
        }

        public override string GetInteractorShortDescription()
        {
            return $"";
        }

        public override bool CanBuild(Block block)
        {
            return true;
        }

        public override void Build(Block block, RaycastHit raycastHit)
        {
            var cameraController = CameraController.Get();
            var blocksPlanet = block.GetConnectedPlanet();

            if (CurrentPlanetController.Get().CurrentPlanet() != blocksPlanet && blocksPlanet.HasPort())
            {
                _currentPlanetController.ChangePlanet(blocksPlanet);
            }
            else
            {
                cameraController.FocusOnPlanet(blocksPlanet);
            }
        }

        public override void OnFailedToBuild(Vector3 hitPoint)
        {
        }

        public override void OnBuilt(Vector3 hitPoint)
        {
        }

        public override void OnSecondaryInteract(Block block, RaycastHit hit)
        {
        }

        public override bool Continuous()
        {
            return false;
        }

        public override bool Hoverable()
        {
            return true;
        }

        public override void Hover(RaycastHit hit)
        {
            OnHover?.Invoke(hit);
        }

        public override string GetCannotBuildHereMessage(Block block)
        {
            // Not called
            return "";
        }

        public override float MaxActivationDistance()
        {
            return 2500f;
        }
    }
}