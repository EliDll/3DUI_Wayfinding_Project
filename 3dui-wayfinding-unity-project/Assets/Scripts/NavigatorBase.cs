using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Navigator
{
    public abstract class NavigatorBase : MonoBehaviour
    {

        [SerializeField] protected Direction currentDirection;
        protected MeshRenderer meshRenderer;
        
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        protected virtual void Start()
        {
            ResetToDefault();
        }

        protected void OnTriggerEnter(Collider other)
        {
            //ChangeDirection(Direction.Right);
        }

        protected void OnTriggerExit(Collider other)
        {
            //ResetToDefault();
        }

        public virtual void ChangeDirection(Direction direction)
        {
            ResetToDefault();
            switch (direction)
            {
                case Direction.Straight:
                    GoStraight();
                    break;
                case Direction.Back:
                    TurnBack();
                    break;
                case Direction.Left:
                    TurnLeft();
                    break;
                case Direction.Right:
                    TurnRight();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public virtual void ResetToDefault()
        {
        }

        public virtual void TurnOn()
        {
        }

        protected virtual void TurnRight()
        {
        }

        protected virtual void TurnLeft()
        {
        }

        protected virtual void TurnBack()
        {
        }

        protected virtual void GoStraight()
        {
        }
    }
}