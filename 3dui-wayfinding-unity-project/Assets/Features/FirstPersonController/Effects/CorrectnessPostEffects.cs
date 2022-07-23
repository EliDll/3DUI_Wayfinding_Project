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

	private int _minSaturation = -50;
	private int _maxSaturation = 0;

    private float _intensityChangeSpeed = 0.1f;
    private Vignette _vignette;
	private ColorAdjustments _colorAdjustments;
    
    // Start is called before the first frame update
    void Start()
    {
        _volume.profile.TryGet<Vignette>(out _vignette);
        _volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
        _vignette.intensity.value = 0f;
		_colorAdjustments.saturation.value = _maxSaturation;
        _characterSignals.Moved.Subscribe(w =>
        {
            if (_characterSignals.IsEffects.Value)
            {
                float newVignette = (1.0f - _hologramPath.CorrectnessScale.Value) * 0.4f;
                _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, newVignette,
                    w.magnitude * _intensityChangeSpeed);
                int newSaturation = (int)Mathf.Lerp(_minSaturation, _maxSaturation, _hologramPath.CorrectnessScale.Value);
                _colorAdjustments.saturation.value = Mathf.Lerp(_colorAdjustments.saturation.value, newSaturation,
                    w.magnitude * _intensityChangeSpeed);
            }
        }).AddTo(this);
        _characterSignals.IsEffects.Where(isEffects => isEffects == false).Subscribe(_ =>
        {
            _vignette.intensity.value = 0f;
            _colorAdjustments.saturation.value = _maxSaturation;
        }).AddTo(this);
    }

    void Awake()
    {
        _characterSignals =
            characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }
}
