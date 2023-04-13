using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Ground")]
    public float CheckGroundRadius;
    public LayerMask GroundLayers;

    [Header("CameraBase")]
    public Transform CharacterTransform;
    public GameObject CinemachineCameraTarget;

    private List<MoveSettings> _moveSettings = new List<MoveSettings>();
    private Dictionary<MoveType, MoveSettings> _moveSettingsDict;
    private MoveSettings _currentMoveSettings;
    private PlayerInputState _inputState;
    private MoveType _moveType = MoveType.Walk;

    private Rigidbody _rb;
    private Animator _anim;
    private GameObject _mainCamera;

    private Vector2 _look;
    private Vector2 _move;

    private Vector2 _inputVector;
    private Vector3 _smoothMoveVector;
    private Vector3 _targetDirection;
    private Quaternion _lookDirection;

    private bool _isGround = false;
    private bool _jump = false;
    private bool _fly = false;

    private float _rotationVelocityX;
    private float _rotationVelocityY;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private int _animIDForward;
    private int _animIDRight;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;


    #region MonoBehaviourFunctions
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _inputState = GetComponent<PlayerInputState>();
        _anim = GetComponentInChildren<Animator>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        AssignAnimationIDs();
        AssembleResources();

        //change settings automatically in upd, based on move type
        _currentMoveSettings = GetSettings(_moveType);

        _jumpTimeoutDelta = _currentMoveSettings.JumpTimeout;
        _fallTimeoutDelta = _currentMoveSettings.FallTimeout;
    }

    private void Update()
    {
        Move();
        Jump();
        GroundedCheck();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
    #endregion


    #region Initialize
    private void AssignAnimationIDs()
    {
        _animIDForward = Animator.StringToHash("Forward");
        _animIDRight = Animator.StringToHash("Right");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void AssembleResources()
    {
        _moveSettings = Resources.LoadAll<MoveSettings>("PlayerMoveSettings").ToList();
        _moveSettingsDict = _moveSettings.ToDictionary(r => r.Reference, r => r);
    }
    #endregion


    #region Main
    private void Jump()
    {
        if (_isGround)
        {
            _fallTimeoutDelta = _currentMoveSettings.FallTimeout;

            _anim.SetBool(_animIDJump, false);
            _anim.SetBool(_animIDFreeFall, false);

            if (_jump)
            {
                float jumpForce = CalculateJumpSpeed(_currentMoveSettings.JumpHeight, Physics.gravity.magnitude);

                _rb.velocity = Vector3.up * jumpForce;

                _anim.SetBool(_animIDJump, true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = _currentMoveSettings.JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _anim.SetBool(_animIDFreeFall, true);
            }

            _jump = false;
        }
    }
    private void Move()
    {
        _inputVector = CalculateInput();
        _smoothMoveVector = CalculateSmoothInput();
        _targetDirection = CalculateDirection();
        _lookDirection = CalculateLookDirection();

        switch (_moveType)
        {
            case MoveType.Walk:
                Walk();
                CharacterWalkRotation();
                break;
            case MoveType.Fly:
                Fly();
                CharacterFlyRotation();
                break;
            default:
                break;
        }
    }
    private void Fly()
    {
        _rb.velocity = new Vector3(
            _targetDirection.x * _currentMoveSettings.Speed * _smoothMoveVector.magnitude,
            _rb.velocity.y,
            _targetDirection.z * _currentMoveSettings.Speed * _smoothMoveVector.magnitude);

        _anim.SetFloat(_animIDRight, _smoothMoveVector.x);
        _anim.SetFloat(_animIDForward, _smoothMoveVector.z);
        _anim.SetFloat(_animIDMotionSpeed, _smoothMoveVector.magnitude);
    }
    private void Walk()
    {
        _rb.velocity = new Vector3(
            _targetDirection.x * _currentMoveSettings.Speed * _smoothMoveVector.magnitude,
            _rb.velocity.y,
            _targetDirection.z * _currentMoveSettings.Speed * _smoothMoveVector.magnitude);

        _anim.SetFloat(_animIDRight, _smoothMoveVector.x);
        _anim.SetFloat(_animIDForward, _smoothMoveVector.z);
        _anim.SetFloat(_animIDMotionSpeed, _smoothMoveVector.magnitude);
    }
    private void CameraRotation()
    {
        CinemachineCameraTarget.transform.rotation = _lookDirection;
    }
    private void CharacterWalkRotation()
    {
        CharacterTransform.rotation = CalculateSlerpYAxisRotation();
    }
    private void CharacterFlyRotation()
    {
        CharacterTransform.rotation = CalculateLookDirection();
    }
    #endregion


    #region VirtualInput
    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        _look = virtualLookDirection;
    }
    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        _move = virtualMoveDirection;
    }
    public void VirtualJumpInput()
    {
        _jump = true;
    }
    public void VirtualFlyInput()
    {
        MoveStateChange(MoveType.Fly);
    }
    public void VirtualWalkInput()
    {
        MoveStateChange(MoveType.Walk);
    }
    private void MoveStateChange(MoveType state)
    {
        switch (state)
        {
            case MoveType.Walk:
                _moveType = MoveType.Walk;
                _inputState.SetState(InputState.MainWalk);
                break;
            case MoveType.Fly:
                _moveType = MoveType.Fly;
                _inputState.SetState(InputState.MainFly);
                break;
            default:
                break;
        }

        _currentMoveSettings = GetSettings(_moveType);
    }
    #endregion


    #region Utility
    private void GroundedCheck()
    {
        Vector3 spherePosition = transform.position;
        _isGround = Physics.CheckSphere(spherePosition,
            CheckGroundRadius,
            GroundLayers,
            QueryTriggerInteraction.Ignore);

        _anim.SetBool(_animIDGrounded, _isGround);
    }
    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }
    private MoveSettings GetSettings(MoveType type)
    {
        return _moveSettingsDict[type];
    }
    private Vector2 CalculateInput()
    {
        return new Vector2(
            Mathf.Clamp(_move.x, _currentMoveSettings.HorizontalLimits.x, _currentMoveSettings.HorizontalLimits.y),
            Mathf.Clamp(_move.y, _currentMoveSettings.VerticalLimits.x, _currentMoveSettings.VerticalLimits.y)
            );
    }
    private Vector3 CalculateSmoothInput()
    {
        return Vector3.Slerp(
            _smoothMoveVector,
            new Vector3(_move.x, 0.0f, _move.y),
            _currentMoveSettings.SmoothSpeedMoment * Time.deltaTime);
    }
    private Vector3 CalculateDirection()
    {
        Vector3 _moveVector = new Vector3(_inputVector.x, 0, _inputVector.y);
        float _targetRotation = Mathf.Atan2(_moveVector.x, _moveVector.z)
            * Mathf.Rad2Deg
            + _mainCamera.transform.eulerAngles.y;
        return (Quaternion.Euler(0.0f, _targetRotation, 0.0f)
            * Vector3.forward).normalized
            * _moveVector.magnitude
            * _smoothMoveVector.magnitude;
    }
    private Quaternion CalculateLookDirection()
    {
        Transform camTargetT = CinemachineCameraTarget.transform;

        float XaxisRotation = _look.x;
        float YaxisRotation = _look.y;

        float yRotation = Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.y,
            camTargetT.eulerAngles.y + XaxisRotation * 10f,
            ref _rotationVelocityY, _currentMoveSettings.CameraSmoothTimer);
        float xRotation = Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.x,
            camTargetT.eulerAngles.x - YaxisRotation * 10f,
            ref _rotationVelocityX, _currentMoveSettings.CameraSmoothTimer);
        //float xRotation = Mathf.Clamp(Mathf.SmoothDampAngle(
        //    camTargetT.eulerAngles.x,
        //    camTargetT.eulerAngles.x - YaxisRotation * 10f,
        //    ref _rotationVelocityX, _currentMoveSettings.CameraSmoothTimer),
        //    _currentMoveSettings.BottomClamp, _currentMoveSettings.TopClamp);

        return Quaternion.Euler(xRotation, yRotation, 0.0f);
    }
    private Quaternion CalculateSlerpYAxisRotation()
    {
        Transform camTargetT = CinemachineCameraTarget.transform;

        float XaxisRotation = _look.x;

        float yRotation = Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.y,
            camTargetT.eulerAngles.y + XaxisRotation * 10f,
            ref _rotationVelocityY, _currentMoveSettings.CameraSmoothTimer);

        return Quaternion.Slerp(
            CharacterTransform.rotation,
            Quaternion.Euler(0.0f, yRotation, 0.0f),
            _currentMoveSettings.CharacterRotationSpeed * Time.deltaTime);
    }
    #endregion
}