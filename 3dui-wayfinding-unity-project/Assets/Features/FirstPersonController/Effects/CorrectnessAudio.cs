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
    
    private float _moveChangeSpeed = 0.1f;
    private float _minMovePitch = 0.4f;
    private float _maxMovePitch = 0.9f;
    private float _currentMovePitch = 0.9f;

    private float _currentLookPitch = 0.1f;
    private float _maxLookPitch = 0.1f;
    private float _minLookPitch = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        _characterSignals.Moved.Subscribe(w =>
        {
            if (_characterSignals.IsEffects.Value)
            {
                float newMovePitch = Mathf.Lerp(_minMovePitch, _maxMovePitch, _hologramPath.CorrectnessScale.Value);
                _currentMovePitch = Mathf.Lerp(_currentMovePitch, newMovePitch,
                    w.magnitude * _moveChangeSpeed);
                _audioSource.pitch = _currentLookPitch + _currentMovePitch;
            }
        }).AddTo(this);
        _characterSignals.IsEffects.Where(isEffects => isEffects == false).Subscribe(_ =>
        {
            _audioSource.pitch = 1f;
        }).AddTo(this);
        _hologramPath.CorrectnessScale.Subscribe(v =>
        {
            if (_characterSignals.IsEffects.Value)
            {
                _currentLookPitch = Mathf.Lerp(_minLookPitch, _maxLookPitch, v);
                _audioSource.pitch = _currentLookPitch + _currentMovePitch;
            }
        }).AddTo(this);
    }

    void Awake()
    {
        _audioSource.pitch = _currentLookPitch + _currentMovePitch;
        _characterSignals =
            characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }
}
