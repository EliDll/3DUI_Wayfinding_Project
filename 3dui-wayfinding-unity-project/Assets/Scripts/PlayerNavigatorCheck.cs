using System;
using System.Collections.Generic;
using Enums;
using Navigator;
using UnityEngine;

public class PlayerNavigatorCheck : MonoBehaviour
{
    [SerializeField] private float colliderRange = 20f;
    [SerializeField] private HologramPath hologramPath;
    [SerializeField] private float maxDistanceToCorner = 10f;

    private void Update()
    {
        foreach (var nav in SearchForNearNavigators())
        {
            var closestPathNode = hologramPath.FindClosestPathNodeForPosition(nav.transform.position, nav.transform.forward);

            Direction direction = closestPathNode.pathAngle switch
            {
                > -45 and < 45 => Direction.Back,
                > 45 and < 135 => Direction.Right,
                > -135 and < -45 => Direction.Left,
                _ => Direction.Straight
            };
            float distanceToCorner = Vector3.Distance(nav.transform.position, closestPathNode.position);
            if (distanceToCorner > maxDistanceToCorner)
            {
                // if there is no corner near or the closest corner is target, we need to use pos angle
                direction = closestPathNode.posAngle switch
                {
                    > -90 and < 90 => Direction.Back,
                    _ => Direction.Straight
                };
            }

            nav.ChangeDirection(direction);
        }
    }

    private List<NavigatorBase> SearchForNearNavigators()
    {
        List<NavigatorBase> navigators = new List<NavigatorBase>();
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, colliderRange, 1 << LayerMask.NameToLayer("Navigator"));
        foreach(var col in colliders)
        {
            NavigatorBase navigator = col.gameObject.GetComponent<NavigatorBase>();
            if (!navigator)
            {
                throw new Exception("Navigator Script not found on an object of layer Navigator");
            }
            navigators.Add(col.gameObject.GetComponent<NavigatorBase>());
        }
        
        return navigators;
    }
}