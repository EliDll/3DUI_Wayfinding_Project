using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public struct MoveInputData
{
    public readonly Vector2 move;
    public readonly bool jump;

    public MoveInputData(Vector2 move, bool jump)
    {
        this.move = move;
        this.jump = jump;
    }
}


/// <summary>
///     Controller that handles the character controls and camera controls of the first person player.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour, ICharacterSignals
{
    [Header("References")]
    [SerializeField] private FirstPersonControllerInput firstPersonControllerInput;
    private CharacterController _characterController;
    private Camera _camera;

    [Header("Locomotion Properties")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float maxVertVelocity = 10f;
    [SerializeField] private float maxFallingVelocity = 30f;
    [SerializeField] private float stickToGroundForceMagnitude = 5f;

    [Header("Look Properties")]
    [Range(-90, 0)][SerializeField] private float minViewAngle = -60f;
    [Range(0, 90)][SerializeField] private float maxViewAngle = 60f;

    // side effects for camera bob
    private Subject<Vector3> _moved;
    public IObservable<Vector3> Moved => _moved;
    
    // side effects for navigation
    private ReactiveProperty<bool> _isEffects;
    public ReactiveProperty<bool> IsEffects => _isEffects;

    private ReactiveProperty<bool> _isRunning;
    public ReactiveProperty<bool> IsRunning => _isRunning;

    private float _strideLength = 2.5f;
    public float StrideLength => _strideLength;

    // side effects for sound
    private Subject<Unit> _landed;
    public IObservable<Unit> Landed => _landed;

    private Subject<Unit> _jumped;
    public IObservable<Unit> Jumped => _jumped;

    private Subject<Unit> _stepped;
    public IObservable<Unit> Stepped => _stepped;

    private float stepDistance = 0f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<Camera>();

        // initialize effects
        _isRunning = new ReactiveProperty<bool>(false);
        _isEffects = new ReactiveProperty<bool>(true);

        _moved = new Subject<Vector3>().AddTo(this);
        _landed = new Subject<Unit>().AddTo(this);
        _jumped = new Subject<Unit>().AddTo(this);

        _stepped = new Subject<Unit>().AddTo(this);

        // Track distance walked to emit step events.
        Moved.Subscribe(w => {
            stepDistance += w.magnitude;
            if (stepDistance > _strideLength)
            {
                _stepped.OnNext(Unit.Default);
            }
            stepDistance %= _strideLength;
        }).AddTo(this);
    }

    private void Start()
    {
        // Ensures the first frame counts as "grounded".
        _characterController.Move(-stickToGroundForceMagnitude * transform.up);
        
        // Move Inputs flip the view at the start. Hacky solution by not applying move input first couple of frames.
        StartCoroutine(StartDelayed());
    }

    private IEnumerator StartDelayed()
    {
        yield return new WaitForSeconds(1f);

        // synchronize the jump stream with the update rate
        var jumpLatch = LatchObservables.Latch(this.UpdateAsObservable(), firstPersonControllerInput.Jump, false);

        // zip jump and move together (zippedData is a new Observable containing MoveInputData objects)
        firstPersonControllerInput.Move.Zip(jumpLatch, (move, jump) => new MoveInputData(move, jump)).Subscribe(HandleMotion).AddTo(this);

        firstPersonControllerInput.Look.Where(v => v != Vector2.zero).Subscribe(HandleLook).AddTo(this);

        firstPersonControllerInput.EnableEffects.Subscribe(s =>
        {
            _isEffects.Value = !_isEffects.Value;
        }).AddTo(this);
    }

    private void HandleMotion(MoveInputData i)
    {
        var wasGrounded = _characterController.isGrounded;

        // Vertical movement:
        var verticalVelocity = 0f;

        if (i.jump && wasGrounded)
        {
            // we are on the ground and want to jump
            verticalVelocity = jumpSpeed;
            _jumped.OnNext(Unit.Default);
        }
        else if (!wasGrounded)
        {
            // we are in the air --> apply gravity
            verticalVelocity = Math.Clamp(_characterController.velocity.y + (Physics.gravity.y * Time.deltaTime * 3.0f), -maxFallingVelocity, maxVertVelocity);
        }
        else
        {
            // we are on the ground and don't want to jump --> nevertheless push us down a little,
            // because that is required for isGrounded to work
            verticalVelocity = -Mathf.Abs(stickToGroundForceMagnitude);
        }

        // Horizontal movement:
        var currentSpeed = walkSpeed;

        // if run button is pressed, overwrite currentSpeed it with run speed
        if (firstPersonControllerInput.Run.Value)
        {
            currentSpeed = runSpeed;
        }

        var horizontalVelocity = i.move * currentSpeed; // Calculate velocity (direction * speed).

        // Combine horizontal and vertical movement.
        var characterVelocity = transform.TransformVector(new Vector3(
            horizontalVelocity.x,
            verticalVelocity,
            horizontalVelocity.y));

        // Apply movement.
        var distance = characterVelocity * Time.deltaTime;
        _characterController.Move(distance);

        var tempIsRunning = false;
        if (wasGrounded && _characterController.isGrounded)
        {
            // Both started and ended this frame on the ground.
            _moved.OnNext(_characterController.velocity * Time.deltaTime);
            if (_characterController.velocity.magnitude > 0)
            {
                // The character is running if the input is active and the character is actually moving on the ground
                tempIsRunning = firstPersonControllerInput.Run.Value;
            }
        }
        _isRunning.Value = tempIsRunning;

        if (!wasGrounded && _characterController.isGrounded)
        {
            // Didn't start on the ground, but ended up there.
            _landed.OnNext(Unit.Default);
        }
    }

    private void HandleLook(Vector2 inputLook)
    {
        // Translate 2D mouse input into euler angle rotations.

        // Horizontal look with rotation around the vertical axis, where + means clockwise.
        var horizontalLook = inputLook.x * Vector3.up * Time.deltaTime;
        transform.localRotation *= Quaternion.Euler(horizontalLook);

        // Vertical look with rotation around the horizontal axis, where + means upwards.
        var verticalLook = inputLook.y * Vector3.left * Time.deltaTime;
        var newQ = _camera.transform.localRotation * Quaternion.Euler(verticalLook);

        _camera.transform.localRotation =
            RotationTools.ClampRotationAroundXAxis(newQ, -maxViewAngle, -minViewAngle);
    }
}