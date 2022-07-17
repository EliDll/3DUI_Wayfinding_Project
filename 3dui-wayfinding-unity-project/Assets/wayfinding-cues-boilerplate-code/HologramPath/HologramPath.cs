using System;
using System.Collections;
using System.Collections.Generic;
using Enums;
using PathFinder;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using Utils;

public class HologramPath : MonoBehaviour
{

    [Header("Colliders Configuration")]
    [SerializeField] private GameObject[] colliders;
    [SerializeField] private float colliderCutoff;
    [SerializeField] private float colliderOffset;
    [SerializeField] private Vector3 inactivePosition = Vector3.zero;
    [SerializeField] private float heightOffset;
    [SerializeField] private float colliderHeight;
    [SerializeField] private float colliderWidth;
    
    private Transform player;
    private Transform target;

    private ReactiveProperty<float> _correctnessScale;
    public ReactiveProperty<float> CorrectnessScale => _correctnessScale;
    
    private float _cornerCutOff = 5f;

    private Vector3 _colliderTransformCorrection;

    private void Awake()
    {
        _correctnessScale = new ReactiveProperty<float>(1.0f);
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        _colliderTransformCorrection = new Vector3(0.0f,heightOffset,0.0f);
        player = PathFinderManager.Instance.player;
        target = PathFinderManager.Instance.target;
    }

    private void OnEnable()
    {
        PathFinder.PathFinderManager.Instance.CurrentPathChangedEvent += HandleCurrentPathChanged;
    }

    private void OnDisable()
    {
        PathFinder.PathFinderManager.Instance.CurrentPathChangedEvent -= HandleCurrentPathChanged;
    }
    
    private void HandleCurrentPathChanged()
    {
        Vector3[] corners = PathFinderManager.Instance.CurrentPath.corners;
        UpdateHologramMarkers(corners);
        UpdateCorectnessScale(corners);
        for (int i = 0; i < corners.Length - 1; i++)
            Debug.DrawLine(corners[i], corners[i + 1], Color.red);
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