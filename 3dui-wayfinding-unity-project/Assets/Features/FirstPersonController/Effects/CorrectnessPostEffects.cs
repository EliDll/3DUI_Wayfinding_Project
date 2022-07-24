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

	private int _minMoveSaturation = -30;
	private int _maxMoveSaturation = 0;
	private float _minMoveVignette = 0f;
	private float _maxMoveVignette = 0.3f;
	private float _currentMoveVignette = 0f;
	private int _currentMoveSaturation = 0;

	private float _currentLookVignette = 0f;
	private int _currentLookSaturation = 0;
	private float _maxLookVignette = 0.1f;
	private int _maxLookSaturation = 0;
	private float _minLookVignette = 0f;
	private int _minLookSaturation = -10;

    private float _moveChangeSpeed = 0.1f;
    private Vignette _vignette;
	private ColorAdjustments _colorAdjustments;
    
    // Start is called before the first frame update
    void Start()
    {
        _volume.profile.TryGet<Vignette>(out _vignette);
        _volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);
		_vignette.intensity.value = _currentMoveVignette + _currentLookVignette;
		_colorAdjustments.saturation.value = _currentMoveSaturation + _currentLookSaturation;
        _characterSignals.Moved.Subscribe(w =>
        {
            if (_characterSignals.IsEffects.Value)
            {
                float newMoveVignette = Mathf.Lerp(_maxMoveVignette, _minMoveVignette, _hologramPath.CorrectnessScale.Value);
                _currentMoveVignette = Mathf.Lerp(_currentMoveVignette, newMoveVignette,
                    w.magnitude * _moveChangeSpeed);
				_vignette.intensity.value = _currentMoveVignette + _currentLookVignette;
                int newMoveSaturation = (int)Mathf.Lerp(_minMoveSaturation, _maxMoveSaturation, _hologramPath.CorrectnessScale.Value);
                _currentMoveSaturation = (int)Mathf.Lerp(_currentMoveSaturation, newMoveSaturation,
                    w.magnitude * _moveChangeSpeed);
				_colorAdjustments.saturation.value = _currentMoveSaturation + _currentLookSaturation;
            }
        }).AddTo(this);
        _hologramPath.CorrectnessScale.Subscribe(v =>
        {
            if (_characterSignals.IsEffects.Value)
            {
                _currentLookVignette = Mathf.Lerp(_maxLookVignette, _minLookVignette, v);
                _currentLookSaturation = (int)Mathf.Lerp(_minLookSaturation, _maxLookSaturation, v);
				_vignette.intensity.value = _currentMoveVignette + _currentLookVignette;
				_colorAdjustments.saturation.value = _currentMoveSaturation + _currentLookSaturation;
            }
        }).AddTo(this);
        _characterSignals.IsEffects.Where(isEffects => isEffects == false).Subscribe(_ =>
        {
            _vignette.intensity.value = 0f;
            _colorAdjustments.saturation.value = 0;
        }).AddTo(this);
    }

    void Awake()
    {
        _characterSignals =
            characterSignalsInterfaceTarget.GetComponent<ICharacterSignals>();
    }
}
