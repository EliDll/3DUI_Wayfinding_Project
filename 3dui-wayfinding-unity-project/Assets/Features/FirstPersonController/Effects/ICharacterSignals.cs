using System;
using UniRx;
using UnityEngine;

public interface ICharacterSignals
{
    float StrideLength { get; }
    ReactiveProperty<bool> IsRunning { get; }
    
    ReactiveProperty<bool> IsEffects { get; }

    IObservable<Vector3> Moved { get; }

    IObservable<Unit> Landed { get; }
    IObservable<Unit> Jumped { get; }
    IObservable<Unit> Stepped { get; }
}
