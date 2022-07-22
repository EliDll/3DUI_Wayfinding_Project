using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPool : Singleton<TargetPool>
{
    [SerializeField] protected GameObject[] targets = new GameObject[0];

    [SerializeField] protected int currentTargetIndex = 0;

    public Transform currentTargetTransform;

    public void Start()
    {
        currentTargetTransform = targets.Length > 0 ? targets[0].transform : this.transform;
        ToggleTargets();
    }

    public void OnTargetReached()
    {
        currentTargetIndex = (currentTargetIndex + 1) % targets.Length;
        currentTargetTransform = targets[currentTargetIndex].transform;
        ToggleTargets();
    }

    private void ToggleTargets()
    {
        //only activate next target
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i].SetActive(i == currentTargetIndex);
        }
    }
}
