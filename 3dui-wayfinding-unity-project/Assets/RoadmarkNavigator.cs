using System;
using Enums;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Navigator
{
    public class RoadmarkNavigator : NavigatorBase
    {
        [SerializeField] private Material turnLeftMaterial;
        [SerializeField] private Material turnRightMaterial;
        [SerializeField] private Material goStraightMaterial;
        [SerializeField] private Material turnBackMaterial;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private double throttleInterval = 1;

        private bool cooldown = false;
        private double timeAcc = 0;

        protected override void Start()
        {
            // base calls ChangeDirection. Must be at the end
            base.Start();
        }

        protected override void Update()
        {
            // base calls ChangeDirection. Must be at the end
            if (cooldown)
            {
                // count
                timeAcc += Time.deltaTime;
                if (timeAcc > throttleInterval)
                {
                    cooldown = false;
                    timeAcc = 0;
                }
            }
            base.Update();
        }

        protected override void TurnBack() {
            if (cooldown) { return; } else { cooldown = true; }
            base.TurnBack();
            meshRenderer.material = turnBackMaterial;
        }

        protected override void GoStraight()
        {
            if (cooldown) { return; } else { cooldown = true; }
            base.GoStraight();
            meshRenderer.material = goStraightMaterial;
        }
        protected override void TurnLeft()
        {
            if (cooldown) { return; } else { cooldown = true; }
            base.TurnLeft();
            meshRenderer.material = turnLeftMaterial;

        }
        protected override void TurnRight()
        {
            if (cooldown) { return; } else { cooldown = true; }
            base.TurnRight();
            meshRenderer.material = turnRightMaterial;
        }

        public override void TurnOn()
        {
            base.TurnOn();
        }

        public override void ResetToDefault()
        {
            if (cooldown) { return; } else { cooldown = true; }
            base.ResetToDefault();
            meshRenderer.material = defaultMaterial;
        }
    }
}