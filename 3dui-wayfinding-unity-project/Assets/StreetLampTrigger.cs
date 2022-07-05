using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetLampTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;

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
        Debug.Log("Entered Collision radius");
        this.targetObject.active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited Collision radius");
        this.targetObject.active = false;
    }
}
