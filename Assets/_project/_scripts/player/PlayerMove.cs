using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private List<MoveSettings> moveSettings = new List<MoveSettings>();
    private Dictionary<MoveType, MoveSettings> moveSettingsDict;
    private MoveSettings _moveSettings;

    [Header("Ground")]
    public float CheckGroundRadius = 0.2f;
    public LayerMask GroundLayers;

    [Header("CameraBase")]
    public Transform CharacterTransform;
    public GameObject CinemachineCameraTarget;

    private Rigidbody rb;
    private Animator anim;
    private GameObject _mainCamera;

    private Vector2 inputVector;
    private Vector2 look;
    private Vector2 move;
    private Vector3 moveVector;

    private bool _isGround = false;
    private bool _jump = false;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        AssignAnimationIDs();
        AssembleResources();

        _moveSettings = GetSettings(MoveType.Walk);

        _jumpTimeoutDelta = _moveSettings.JumpTimeout;
        _fallTimeoutDelta = _moveSettings.FallTimeout;
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

    private void Jump()
    {
        if (_isGround)
        {
            _fallTimeoutDelta = _moveSettings.FallTimeout;

            anim.SetBool(_animIDJump, false);
            anim.SetBool(_animIDFreeFall, false);

            if (_jump)
            {
                float jumpForce = CalculateJumpSpeed(_moveSettings.JumpHeight, Physics.gravity.magnitude);

                rb.velocity = Vector3.up * jumpForce;

                anim.SetBool(_animIDJump, true);
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = _moveSettings.JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                anim.SetBool(_animIDFreeFall, true);
            }

            _jump = false;
        }
    }

    private void Move()
    {
        inputVector = new Vector2(
            Mathf.Clamp(move.x, _moveSettings.HorizontalLimits.x, _moveSettings.HorizontalLimits.y),
            Mathf.Clamp(move.y, _moveSettings.VerticalLimits.x, _moveSettings.VerticalLimits.y)
            );

        Vector3 _moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        moveVector = Vector3.Slerp(
            moveVector,
            new Vector3(move.x, 0.0f, move.y),
            _moveSettings.SmoothSpeedMoment * Time.deltaTime);

        float _targetRotation = Mathf.Atan2(_moveVector.x, _moveVector.z)
            * Mathf.Rad2Deg
            + _mainCamera.transform.eulerAngles.y;
        Vector3 targetDirection = 
            (Quaternion.Euler(0.0f, _targetRotation, 0.0f)
            * Vector3.forward).normalized
            * _moveVector.magnitude
            * moveVector.magnitude;

        rb.velocity = new Vector3(
            targetDirection.x * _moveSettings.Speed * moveVector.magnitude,
            rb.velocity.y,
            targetDirection.z * _moveSettings.Speed * moveVector.magnitude);

        anim.SetFloat(_animIDRight, moveVector.x);
        anim.SetFloat(_animIDForward, moveVector.z);
        anim.SetFloat(_animIDMotionSpeed, moveVector.magnitude);
    }

    private void CameraRotation()
    {
        Transform camTargetT = CinemachineCameraTarget.transform;

        float XaxisRotation = look.x;
        float YaxisRotation = look.y;

        float yRotation = Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.y,
            camTargetT.eulerAngles.y + XaxisRotation * 10f, 
            ref _rotationVelocityY, _moveSettings.CameraSmoothTimer);
        float xRotation = Mathf.Clamp(Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.x,
            camTargetT.eulerAngles.x - YaxisRotation * 10f, 
            ref _rotationVelocityX, _moveSettings.CameraSmoothTimer),
            _moveSettings.BottomClamp, _moveSettings.TopClamp);

        camTargetT.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        CharacterTransform.rotation = Quaternion.Slerp(
            CharacterTransform.rotation, 
            Quaternion.Euler(0.0f, yRotation, 0.0f),
            _moveSettings.CharacterRotationSpeed * Time.deltaTime);
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        look = virtualLookDirection;
    }

    public void VirtualMoveInput(Vector2 virtualMoveDirection)
    {
        move = virtualMoveDirection;
    }

    public void VirtualJumpInput()
    {
        _jump = true;
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = transform.position;
        _isGround = Physics.CheckSphere(spherePosition,
            CheckGroundRadius,
            GroundLayers,
            QueryTriggerInteraction.Ignore);

        anim.SetBool(_animIDGrounded, _isGround);
    }

    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    private void AssignAnimationIDs()
    {
        _animIDForward      = Animator.StringToHash("Forward");
        _animIDRight        = Animator.StringToHash("Right");
        _animIDGrounded     = Animator.StringToHash("Grounded");
        _animIDJump         = Animator.StringToHash("Jump");
        _animIDFreeFall     = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed  = Animator.StringToHash("MotionSpeed");
    }

    private void AssembleResources()
    {
        moveSettings = Resources.LoadAll<MoveSettings>("PlayerMoveSettings").ToList();
        moveSettingsDict = moveSettings.ToDictionary(r => r.Reference, r => r);
    }

    public MoveSettings GetSettings(MoveType type) => moveSettingsDict[type];
}