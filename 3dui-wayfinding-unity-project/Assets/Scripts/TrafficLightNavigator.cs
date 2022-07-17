using System;
using System.Collections;
using Enums;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Navigator
{
    public class TrafficLightNavigator : NavigatorBase
    {
        [SerializeField] private TrafficLightSingleNavigator Green;
        [SerializeField] private TrafficLightSingleNavigator Yellow;
        [SerializeField] private TrafficLightSingleNavigator Red;
        private float _time;
        protected override void Start()
        {
            base.Start();
            _time = Time.time;

        }

        public override void ChangeDirection(Direction direction)
        {
            // Yellow light before changing
            base.ChangeDirection(direction);

            /*StartCoroutine(YellowTimer(() =>
            {
                base.ChangeDirection(direction);
            }));*/
        }

        protected override void GoStraight()
        {
            base.GoStraight();
            Green.TurnOn();
            Green.ChangeDirection(Direction.Straight);
        }

        protected override void TurnBack()
        {
            base.TurnBack();
            Red.TurnOn();
        }

        protected override void TurnRight()
        {
            base.TurnRight();
            Green.TurnOn();
            Green.ChangeDirection(Direction.Right);
        }

        protected override void TurnLeft()
        {
            base.TurnLeft();
            Green.TurnOn();
            Green.ChangeDirection(Direction.Left);
        }

        public override void ResetToDefault()
        {
            base.ResetToDefault();
            Green.ResetToDefault();
            Red.ResetToDefault();
            Yellow.ResetToDefault();
        }

        IEnumerator YellowTimer(Action callback)
        {
            Yellow.TurnOn();
            yield return new WaitForSeconds(2f);
            callback.Invoke();
            Yellow.ResetToDefault();
        }
    }
}