using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using PathFinder;
using UnityEngine;

namespace Navigator
{
    public struct PathNodeAngle
    {
        public Vector3 position;
        // angle of the Vector from this node to next on the path.
        public float pathAngle;
        // angle between navigator position and closest corner
        public float posAngle;

        public PathNodeAngle(Vector3 position, float pathAngle, float posAngle)
        {
            this.position = position;
            this.pathAngle = pathAngle;
            this.posAngle = posAngle;
        }
    }
    
    public abstract class NavigatorBase : MonoBehaviour
    {
        [SerializeField] private float maxDistanceToCorner = 10f;
        [SerializeField] private float colliderRange = 20f;

        [SerializeField] protected Direction currentDirection;
        protected MeshRenderer meshRenderer;

        private Transform _player;
        private bool _active = false;
        
        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        protected virtual void Start()
        {
            ResetToDefault();
            _player = PathFinderManager.Instance.player;
        }

        protected virtual void Update()
        {
            UpdateActivation();
        }

        private void HandleCurrentPathChanged()
        {
            UpdateDirectionOfNavigator();
        }
        
        private void UpdateActivation()
        {
            float dist = Vector3.Distance(_player.position, transform.position);
            
            // Player entered range
            if (dist < colliderRange && !_active)
            {
                _active = true;
                PathFinderManager.Instance.CurrentPathChangedEvent += HandleCurrentPathChanged;
            }
            // Player exited range
            else if(dist > colliderRange && _active)
            {
                _active = false;
                PathFinderManager.Instance.CurrentPathChangedEvent -= HandleCurrentPathChanged;
                ResetToDefault();
            }
        }

        #region Direction Computing
        private void UpdateDirectionOfNavigator()
        {
            var position = transform.position;
            var closestPathNode = FindClosestPathNodeAngleForPosition(position, transform.forward);

            Direction direction = closestPathNode.pathAngle switch
            {
                > -45 and < 45 => Direction.Back,
                > 45 and < 135 => Direction.Right,
                > -135 and < -45 => Direction.Left,
                _ => Direction.Straight
            };
        
            float distanceToCorner = Vector3.Distance(position, closestPathNode.position);
            if (distanceToCorner > maxDistanceToCorner)
            {
                // if there is no corner near or the closest corner is target, we need to use pos angle
                direction = closestPathNode.posAngle switch
                {
                    > -45 and < 45 => Direction.Back,
                    > 45 and < 135 => Direction.Right,
                    > -135 and < -45 => Direction.Left,
                    _ => Direction.Straight
                };
            }

            ChangeDirection(direction);
        }
        
        private int FindClosestCorner(Vector3 position)
        {
            Vector3[] corners = PathFinderManager.Instance.CurrentPath.corners;
            float closestDist = Single.PositiveInfinity;
            int closestPosIx = 0;
            
            for (int cornerIx = 0; cornerIx < corners.Length; cornerIx++)
            {
                Vector3 corner = corners[cornerIx];
                if (Vector3.Distance(position, corner) < closestDist)
                {
                    closestPosIx = cornerIx;
                    closestDist = Vector3.Distance(position, corner);
                }
            }
    
            return closestPosIx;
        }
    
        private PathNodeAngle FindClosestPathNodeAngleForPosition(Vector3 position, Vector3 forward)
        {
            PathNodeAngle pathNodeAngle = new PathNodeAngle(Vector3.zero, 0, 0);
            Vector3[] corners = PathFinder.PathFinderManager.Instance.CurrentPath.corners;
            
            if (corners.Length == 0)
            {
                return pathNodeAngle;
            }
    
            int closestPosIx = FindClosestCorner(position);

            if (closestPosIx == 0)
            {
                // we are not interested in the angle of the first vector
                closestPosIx++;
            }
            
            pathNodeAngle.position = corners[closestPosIx];
            pathNodeAngle.posAngle = Utils.Utils.AngleSigned(position, pathNodeAngle.position, forward);
            
            if (closestPosIx >= corners.Length - 1)
            {
                // closest corner is the target, direction can be assigned directly.
                pathNodeAngle.pathAngle = pathNodeAngle.posAngle;
                return pathNodeAngle;
            }
    
            Vector3 first = corners[closestPosIx];
            Vector3 last = corners[closestPosIx + 1];
            
            // Angle is calculated on xz plane.
            pathNodeAngle.pathAngle = Utils.Utils.AngleSigned(first, last, forward);
    
            return pathNodeAngle;
        }
        
        #endregion

        #region Virtual Functions
        public virtual void ChangeDirection(Direction direction)
        {
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
        #endregion

    }
}