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
            var posAndDir = hologramPath.GetClosestPathNodeAndDirectionForPosition(nav.transform.position);
            Vector3 nearestCorner = posAndDir.Item1;
            Direction nearestCornerDirection = posAndDir.Item2;
            if (Vector3.Distance(nav.transform.position, nearestCorner) > maxDistanceToCorner)
            {
                // if there is no corner near, just make it straight
                nav.ChangeDirection(Direction.Straight);
            }
            else
            {
                nav.ChangeDirection(nearestCornerDirection);
            }
            
            // TODO: Change direction according to local direction of navigator
            // TODO: Change direction according to position of player. (being back)
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