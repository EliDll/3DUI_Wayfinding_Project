using PathFinder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class GiantArrowCompass: MonoBehaviour
{
    private Transform _player;
    private ICharacterSignals _characterSignals;

    private void Awake()
    {
        _player = PathFinderManager.Instance.player;
        _characterSignals = _player.GetComponent<ICharacterSignals>();
    }

    protected virtual void Start()
    {
        _characterSignals.IsEffects.Subscribe(isEffects => { this.gameObject.active = !isEffects; }).AddTo(this);
    }

    private void Update()
    {
        this.transform.LookAt(TargetPool.Instance.currentTargetTransform);
    }
}