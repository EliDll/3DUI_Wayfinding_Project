using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;

public class HologramPath : MonoBehaviour
{
    [Header("Marker Configuration")]
    [SerializeField] private GameObject[] markers;
    [SerializeField] private Vector3 inactivePosition;
    [SerializeField] private float markerDistance = 5.0f;
    [SerializeField] private int skipAFewMarkers = 2;
    [Header("Agents Configuration")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    [SerializeField] private float hologramHeightCorrection = 3;

    private NavMeshPath _currentPath;
    private NavMeshAgent _navAgent;
    private Vector3 hologramCorrVec;

    // Start is called before the first frame update
    private void Start()
    {
        _currentPath = new NavMeshPath();
        hologramCorrVec = new Vector3(0.0f, hologramHeightCorrection, 0.0f);
    }

    // Update is called once per frame
    private void Update()
    {
        NavMeshHit playerNavMeshPos;
        NavMeshHit targetNavMeshPos;

        NavMesh.SamplePosition(player.position, out playerNavMeshPos, 6.0f, NavMesh.AllAreas);
        NavMesh.SamplePosition(target.position, out targetNavMeshPos, 6.0f, NavMesh.AllAreas);

        if (playerNavMeshPos.hit && targetNavMeshPos.hit && NavMesh.CalculatePath(playerNavMeshPos.position, targetNavMeshPos.position, NavMesh.AllAreas, _currentPath))
        {
            UpdateHologramMarkers(_currentPath.corners);
            for (int i = 0; i < _currentPath.corners.Length - 1; i++)
                Debug.DrawLine(_currentPath.corners[i], _currentPath.corners[i + 1], Color.red);
        }
    }

    private void UpdateHologramMarkers(Vector3[] path)
    {
        if (path.Length < 2)
        {
            return;
        }

        var potentialMarkerPositions = new List<Vector3>();
        var rest = 0.0f;
        var from = Vector3.zero;
        var to = Vector3.zero;

        //loop over path pairs (i,i+1)
        for (int i = 0; i < path.Length-1; i++)
        {
            from = path[i];
            to = path[i + 1];
            float len = Vector3.Distance(from, to);
            float accumulatedDistance = len + rest;

            // from + (to - from).normalized = direction vector
            // 
            if(accumulatedDistance > markerDistance)
            {
                float lineSegment = markerDistance - rest;
                potentialMarkerPositions.Add(from + (to - from).normalized * lineSegment);
                rest = len - lineSegment;
            }
            else
            {
                // overwrite accumulator
                rest = accumulatedDistance;
            }
        }

        for(int i = 0; i < markers.Length; i++)
        {
            int skipIndex = i + skipAFewMarkers;
            if (potentialMarkerPositions.Count > skipIndex)
            {
                markers[i].transform.position = potentialMarkerPositions[skipIndex];
                markers[i].transform.position += hologramCorrVec;
            }
            else
            {
                markers[i].transform.position = inactivePosition;
            }
        }
    }
}