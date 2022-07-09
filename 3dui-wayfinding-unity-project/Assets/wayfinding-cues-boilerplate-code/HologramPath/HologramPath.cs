using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

public class HologramPath : MonoBehaviour
{
    [Header("Agents Configuration")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;

    [Header("Colliders Configuration")]
    [SerializeField] private GameObject[] colliders;
    [SerializeField] private float colliderCutoff;
    [SerializeField] private float colliderOffset;
    [SerializeField] private Vector3 inactivePosition = Vector3.zero;
    [SerializeField] private float heightOffset;
    [SerializeField] private float colliderHeight;
    [SerializeField] private float colliderWidth;

    private ReactiveProperty<float> _correctnessScale;
    
    public ReactiveProperty<float> CorrectnessScale => _correctnessScale;
    
    private float _cornerCutOff = 5f;

    private NavMeshPath _currentPath;
    private NavMeshAgent _navAgent;
    private Vector3 _colliderTransformCorrection;

    private void Awake()
    {
        _correctnessScale = new ReactiveProperty<float>(1.0f);
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _currentPath = new NavMeshPath();
        _colliderTransformCorrection = new Vector3(0.0f,heightOffset,0.0f);
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
            UpdateCorectnessScale(_currentPath.corners);
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

        Vector3 from;
        Vector3 to;
        Vector3 segVec;
        Vector3 offsetVec;

        float segLen;
        float collLen;

        float lenAcc = 0;
        int iColl = 0;

        for (int i = 0; i < path.Length-1; i++)
        {
            if(iColl < colliders.Length)
            {
                from = path[i];
                to = path[i + 1];
                segLen = Vector3.Distance(from, to);
                segVec = to - from;
                lenAcc += segLen;
                var targetColl = colliders[iColl];

                //first & last collider
                if (lenAcc > colliderOffset && lenAcc - segLen < colliderOffset && lenAcc > colliderCutoff && lenAcc - segLen < colliderCutoff)
                {
                    // remainder of colliderOffset belonging to this segment
                    float startOffset = colliderOffset - (lenAcc - segLen);
                    // remaining valid length for this segment
                    float maxLen = (colliderCutoff - (lenAcc - segLen)) - startOffset;
                    offsetVec = Vector3.Normalize(segVec) * (startOffset + maxLen / 2.0f);
                    collLen = maxLen;
                }
                //first collider
                else if(lenAcc > colliderOffset && lenAcc - segLen < colliderOffset)
                {
                    float validLen = lenAcc - colliderOffset; // guaranteed to be < len and > 0
                    offsetVec = Vector3.Normalize(segVec) * (segLen - validLen / 2.0f);
                    collLen = validLen; // update scale len
                }
                //last collider
                else if (lenAcc > colliderCutoff && lenAcc - segLen < colliderCutoff)
                {
                    float maxLen = colliderCutoff - (lenAcc - segLen);
                    offsetVec = Vector3.Normalize(segVec) * (maxLen / 2.0f);
                    collLen = maxLen; // update scale len
                }
                // middle collider
                else if(lenAcc > colliderOffset && lenAcc < colliderCutoff)
                {
                    offsetVec = segVec / 2.0f;
                    collLen = segLen;
                }
                else
                {
                    // ignore this path segment as it is either entirely too close or too far
                    continue;
                }

                targetColl.transform.localScale = new Vector3(colliderWidth, colliderHeight, collLen);
                targetColl.transform.position = from + offsetVec;
                targetColl.transform.LookAt(to);
                // do world coord. frame corrections after rotation
                targetColl.transform.position += _colliderTransformCorrection;
                iColl++;
            }
        }

        // reset all colliders that are not needed
        for (int j = iColl; j < colliders.Length; j++)
        {
            colliders[j].transform.position = inactivePosition;
        }
    }

    private void UpdateCorectnessScale(Vector3[] path)
    {
        Vector3 targetPoint;

        if (path.Length < 2)
        {
            targetPoint = target.position;
        }

        Vector3 firstCorner = path[0];
        Vector3 secondCorner = path[1];
        float dist = Vector3.Distance(player.position, firstCorner);
        
        if (dist < _cornerCutOff)
        {
            targetPoint = Vector3.Lerp(secondCorner, firstCorner, dist / _cornerCutOff);
        }
        else
        {
            targetPoint = firstCorner;
        }

        Vector3 targetDirection = targetPoint - player.position;
        targetDirection.y = 0;
        _correctnessScale.Value = Vector3.Dot(player.forward, targetDirection.normalized) * 0.5f + 0.5f;
    }
}