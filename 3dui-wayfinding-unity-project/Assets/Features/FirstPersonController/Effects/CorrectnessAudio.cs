using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;

public class CorrectnessAudio : MonoBehaviour
{
    [SerializeField]
    private GameObject characterSignalsInterfaceTarget;
    [SerializeField]
    private HologramPath _hologramPath;
    private ICharacterSignals _characterSignals;
    
    [SerializeField] private AudioSource _audioSource;
    
    private float _intensityChangeSpeed = 0.1f;

    private float _startPitch = 1f;

    private float _lowPitch = 0.5f;
    
    // Start is called before the first frame update
    void Start()
    {
        _characterSignals.Moved.Subscribe(w =>
        {
            if (_characterSignals.IsEffects.Value)
            {
                float scaledCorrectness =
                    (_hologramPath.CorrectnessScale.Value * (_startPitch - _lowPitch)) + _lowPitch;
                _audioSource.pitch = Mathf.Lerp(_audioSource.pitch, scaledCorrectness,
                    w.magnitude * _intensityChangeSpeed);
            }
        }).AddTo(this);
        _characterSignals.IsEffects.Where(isEffects => isEffects == false).Subscribe(_ =>
        {
            _audioSource.pitch = _startPitch;
        }).AddTo(this);
    }

    void Awake()
    {
        _audioSource.pitch = _startPitch;
        _characterSignals =
            characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }
}
