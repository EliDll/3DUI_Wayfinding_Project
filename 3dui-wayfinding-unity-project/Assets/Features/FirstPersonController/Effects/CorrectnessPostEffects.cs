using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;

public class CorrectnessPostEffects : MonoBehaviour
{
    [SerializeField]
    private GameObject characterSignalsInterfaceTarget;
    [SerializeField]
    private HologramPath _hologramPath;
    private ICharacterSignals _characterSignals;
    [SerializeField] private Volume _volume;

    private float _intensityChangeSpeed = 0.1f;
    private Vignette _vignette;
    
    // Start is called before the first frame update
    void Start()
    {
        _volume.profile.TryGet<Vignette>(out _vignette);
        _vignette.intensity.value = 0f;
        _characterSignals.Moved.Subscribe(w =>
        {
            float scaledCorrectness = (1.0f - _hologramPath.CorrectnessScale.Value) * 0.5f;
            _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, scaledCorrectness,
                w.magnitude * _intensityChangeSpeed);
        }).AddTo(this);
    }

    void Awake()
    {
        _characterSignals =
            characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }
}
