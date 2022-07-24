using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


public class InputActionBasedFirstPersonControllerInput : FirstPersonControllerInput
{
    public override IObservable<Vector2> Move => _move;
    private IObservable<Vector2> _move;

    public override IObservable<Vector2> Look => _look;
    private IObservable<Vector2> _look;

    public override IObservable<Unit> Jump => _jump;
    private Subject<Unit> _jump;

    public override ReadOnlyReactiveProperty<bool> Run => _run;
    private ReadOnlyReactiveProperty<bool> _run;

    public override IObservable<Unit> EnableEffects => _enableEffects;
    private Subject<Unit> _enableEffects;

    [Header("Look Properties")]
    [SerializeField] private float lookSmoothingFactor = 14.0f;

    private FirstPersonInputAction _controls;

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    protected void Awake()
    {
        DivideVector2ByDeltaTimeProcessor.Initialize();
        
        _controls = new FirstPersonInputAction();

        // Hide the mouse cursor and lock it in the game window.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Move:
        _move = this.UpdateAsObservable()
            .Select(_ => _controls.Character.Move.ReadValue<Vector2>());

        // Look:
        var smoothLookValue = new Vector2(0, 0);
        _look = this.UpdateAsObservable()
            .Select(_ =>
            {
                var rawLookValue = _controls.Character.Look.ReadValue<Vector2>();

                smoothLookValue = new Vector2(
                    Mathf.Lerp(smoothLookValue.x, rawLookValue.x, lookSmoothingFactor * Time.deltaTime),
                    Mathf.Lerp(smoothLookValue.y, rawLookValue.y, lookSmoothingFactor * Time.deltaTime)
                );

                return smoothLookValue;
            });

        _run = this.UpdateAsObservable().Select(_ => _controls.Character.Run.ReadValueAsObject() != null).ToReadOnlyReactiveProperty();

        _jump = new Subject<Unit>().AddTo(this);
        _controls.Character.Jump.performed += context => {
            _jump.OnNext(Unit.Default);
        };
        
        _enableEffects = new Subject<Unit>().AddTo(this);
        _controls.Character.EnableEffects.performed += context => {
            _enableEffects.OnNext(Unit.Default);
        };
    }
}