using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public Vector2 HorizontalLimits;
    public Vector2 VerticalLimits;

    [Header("Move")]
    public float Speed = 4.5f;
    public float SmoothSpeedMoment = 3f;

    [Header("Jump")]
    public float JumpHeight = 5f;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    [Header("Ground")]
    public float CheckGroundRadius = 0.2f;
    public LayerMask GroundLayers;

    [Header("Camera")]
    public Transform CharacterTransform;
    public GameObject CinemachineCameraTarget;
    public float CharacterRotationSpeed = 5f;
    public float CameraSmoothTimer = 0.15f;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;

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

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
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
            _fallTimeoutDelta = FallTimeout;

            anim.SetBool(_animIDJump, false);
            anim.SetBool(_animIDFreeFall, false);

            if (_jump)
            {
                float jumpForce = CalculateJumpSpeed(JumpHeight, Physics.gravity.magnitude);

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
            _jumpTimeoutDelta = JumpTimeout;

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
            Mathf.Clamp(move.x, HorizontalLimits.x, HorizontalLimits.y),
            Mathf.Clamp(move.y, VerticalLimits.x, VerticalLimits.y)
            );

        Vector3 _moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        moveVector = Vector3.Slerp(
            moveVector,
            new Vector3(move.x, 0.0f, move.y),
            SmoothSpeedMoment * Time.deltaTime);

        float _targetRotation = Mathf.Atan2(_moveVector.x, _moveVector.z)
            * Mathf.Rad2Deg
            + _mainCamera.transform.eulerAngles.y;
        Vector3 targetDirection = 
            (Quaternion.Euler(0.0f, _targetRotation, 0.0f)
            * Vector3.forward).normalized
            * _moveVector.magnitude
            * moveVector.magnitude;

        rb.velocity = new Vector3(
            targetDirection.x * Speed * moveVector.magnitude,
            rb.velocity.y,
            targetDirection.z * Speed * moveVector.magnitude);

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
            ref _rotationVelocityY, CameraSmoothTimer);
        float xRotation = Mathf.Clamp(Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.x,
            camTargetT.eulerAngles.x - YaxisRotation * 10f, 
            ref _rotationVelocityX, CameraSmoothTimer), 
            BottomClamp, TopClamp);

        camTargetT.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        CharacterTransform.rotation = Quaternion.Slerp(
            CharacterTransform.rotation, 
            Quaternion.Euler(0.0f, yRotation, 0.0f), 
            CharacterRotationSpeed * Time.deltaTime);
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
}
