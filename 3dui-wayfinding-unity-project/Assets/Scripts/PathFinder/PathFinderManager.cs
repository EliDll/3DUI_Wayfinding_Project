using System;
using UnityEngine;
using UnityEngine.AI;

namespace PathFinder
{
    public class PathFinderManager : Singleton<PathFinderManager>
    {
        public Transform player;

        public delegate void CurrentPathChangedDelegate();
        public event CurrentPathChangedDelegate CurrentPathChangedEvent;

        public NavMeshPath CurrentPath { get; private set; }
        private NavMeshAgent _navAgent;

        private void Start()
        {
            CurrentPath = new NavMeshPath();
        }

        private void Update()
        {
            NavMesh.SamplePosition(player.position, out var playerNavMeshPos, 6.0f, NavMesh.AllAreas);
            NavMesh.SamplePosition(TargetPool.Instance.currentTargetTransform.position, out var targetNavMeshPos, 6.0f, NavMesh.AllAreas);

            if (playerNavMeshPos.hit && targetNavMeshPos.hit && NavMesh.CalculatePath(playerNavMeshPos.position, targetNavMeshPos.position, NavMesh.AllAreas, CurrentPath))
            {
                if (CurrentPathChangedEvent != null)
                {
                    CurrentPathChangedEvent.Invoke();
                }
            }
        }
    }
}