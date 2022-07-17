using System;
using Enums;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Navigator
{
    public class TrafficLightSingleNavigator : NavigatorBase
    {
        [SerializeField] private Material turnLeftMaterial;
        [SerializeField] private Material turnRightMaterial;
        [SerializeField] private Material goStraightMaterial;
        [SerializeField] private Material turnBackMaterial;

        protected override void Start()
        {
            // base calls ChangeDirection. Must be at the end
            base.Start();
        }
        
        // Don't do anything in update. It's handled by the parent.
        protected override void Update()
        {
            
        }

        protected override void TurnBack()
        {
            base.TurnBack();
            meshRenderer.material = turnBackMaterial;
        }

        protected override void GoStraight()
        {
            base.GoStraight();
            meshRenderer.material = goStraightMaterial;
        }
        protected override void TurnLeft()
        {
            base.TurnLeft();
            meshRenderer.material = turnLeftMaterial;

        }
        protected override void TurnRight()
        {
            base.TurnRight();
            meshRenderer.material = turnRightMaterial;
        }

        public override void TurnOn()
        {
            base.TurnOn();
            meshRenderer.material.SetInt("_On", 1);
        }

        public override void ResetToDefault()
        {
            base.ResetToDefault();
            meshRenderer.material.SetInt("_On", 0);
        }
    }
}