using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Joystick joystick;
    private Rigidbody rb;
    private Animator anim;
    private GameObject _mainCamera;
    private Vector2 inputVector;
    private Vector2 look;

    public Vector2 horizontalLimits;
    public Vector2 verticalLimits;

    [Header("Main")]
    public float speed = 3;
    private float _rotationVelocity;

    [Header("Camera")]
    public GameObject CinemachineCameraTarget;
    public float RotationSmoothTime = 0.12f;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;

    [Header("Jump")]
    private bool isGround = false;
    private bool jump = false;
    public float JumpHeight = 5f;
    public float checkGroundRadius = 0.2f;
    public LayerMask GroundLayers;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

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
        joystick = FindObjectOfType<Joystick>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
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
        if (isGround)
        {
            _fallTimeoutDelta = FallTimeout;

            anim.SetBool(_animIDJump, false);
            anim.SetBool(_animIDFreeFall, false);

            if (jump)
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
            // reset the jump timeout timer
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                anim.SetBool(_animIDFreeFall, true);
            }

            // if we are not grounded, do not jump
            jump = false;
        }
        
    }

    

    private void Move()
    {
        inputVector = new Vector2(
            Mathf.Clamp(joystick.Horizontal, horizontalLimits.x, horizontalLimits.y),
            Mathf.Clamp(joystick.Vertical, verticalLimits.x, verticalLimits.y)
            );

        Vector3 moveVector = new Vector3(inputVector.x, 0, inputVector.y);
        float _targetRotation = Mathf.Atan2(moveVector.x, moveVector.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
        Vector3 targetDirection = (Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward).normalized * moveVector.magnitude;

        rb.velocity = new Vector3(
            targetDirection.x * speed, 
            rb.velocity.y,
            targetDirection.z * speed);

        anim.SetFloat(_animIDRight, joystick.Horizontal);
        anim.SetFloat(_animIDForward, joystick.Vertical);
        anim.SetFloat(_animIDMotionSpeed, joystick.Direction.magnitude);
    }

    private void CameraRotation()
    {
        float XaxisRotation = look.x;
        float YaxisRotation = look.y;
        Vector3 euler = new Vector3(-YaxisRotation, 0, 0);
        CinemachineCameraTarget.transform.eulerAngles += euler;

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            transform.eulerAngles.y + XaxisRotation * 10f,
            ref _rotationVelocity,
            RotationSmoothTime);
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    public void VirtualLookInput(Vector2 virtualLookDirection)
    {
        look = virtualLookDirection;
    }

    public void VirtualJumpInput()
    {
        jump = true;
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = transform.position;
        isGround = Physics.CheckSphere(spherePosition, checkGroundRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        anim.SetBool(_animIDGrounded, isGround);
    }

    private float CalculateJumpSpeed(float jumpHeight, float gravity)
    {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    private void AssignAnimationIDs()
    {
        _animIDForward = Animator.StringToHash("Forward");
        _animIDRight = Animator.StringToHash("Right");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}
