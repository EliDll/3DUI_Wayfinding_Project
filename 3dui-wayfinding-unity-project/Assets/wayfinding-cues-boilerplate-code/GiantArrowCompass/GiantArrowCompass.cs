using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantArrowCompass: MonoBehaviour
{


    private void Update()
    {
        this.transform.LookAt(TargetPool.Instance.currentTargetTransform);
    }
}