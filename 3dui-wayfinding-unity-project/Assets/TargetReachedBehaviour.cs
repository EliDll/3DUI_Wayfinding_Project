using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetReachedBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.transform == TargetPool.Instance.currentTargetTransform)
        {
            Debug.Log("player collision w/ target");
            TargetPool.Instance.OnTargetReached();
        }
    }
}