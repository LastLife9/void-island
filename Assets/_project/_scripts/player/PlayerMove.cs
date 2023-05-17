using System.Collections;
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
    public Transform camRoot;
    public GameObject cam_Walk;
    public GameObject cam_Fly;

    [Header("LookTargets")]
    public float TargetLength = 20f;
    public float TakeCollectablesRadius = 3f;
    public LayerMask TargetLayer;
    public LayerMask CollectableLayer;

    [Header("Attack")]
    public float Damage = 0.4f;
    public float Cooldown = 0.3f;
    public float SwingTimeout = 0.25f;
    public float AttackTimeout = 0.75f;

    private List<MoveSettings> _moveSettings = new List<MoveSettings>();
    private Dictionary<MoveType, MoveSettings> _moveSettingsDict;
    private MoveSettings _currentMoveSettings;
    private PlayerInputState _inputState;

    private MoveType _moveType = MoveType.Walk;
    private MoveType _prevMoveType;

    private Transform _transform;
    private Rigidbody _rb;
    private Animator _anim;
    private GameObject _mainCamera;
    private Camera _cam;

    private Vector2 _look;
    private Vector2 _move;

    private Vector2 _inputVector;
    private Vector3 _smoothMoveVector;
    private Vector3 _targetDirection;
    private Quaternion _lookDirection;

    private bool _isGround = false;
    private bool _jump = false;
    private bool _attack = false;

    private float _rotationVelocityX;
    private float _rotationVelocityY;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _swingTimeoutDelta;
    private float _attackTimeoutDelta;
    private float _attackCooldownDelta;

    private int _animIDForward;
    private int _animIDRight;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    private int _animIDFly;
    private int _animIDAttack;


    #region MonoBehaviourFunctions
    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody>();
        _inputState = GetComponent<PlayerInputState>();
        _anim = GetComponentInChildren<Animator>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _cam = _mainCamera.GetComponent<Camera>();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        AssignAnimationIDs();
        AssembleResources();

        //change settings automatically in upd, based on move type
        MoveStateChange(MoveType.Walk);

        _jumpTimeoutDelta = _currentMoveSettings.JumpTimeout;
        _fallTimeoutDelta = _currentMoveSettings.FallTimeout;
    }

    private void Update()
    {
        Move();
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
        _animIDFly = Animator.StringToHash("Fly");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _animIDAttack = Animator.StringToHash("Attack");
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
    private void Attack()
    {
        if(_attackCooldownDelta >= 0.0f) _attackCooldownDelta -= Time.deltaTime;

        if (_attack)
        {
            _attackTimeoutDelta = AttackTimeout;

            _anim.SetBool(_animIDAttack, true);

            if (_swingTimeoutDelta >= 0)
            {
                _swingTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                Transform target = LookTarget();

                if(target != null)
                {
                    if(target.TryGetComponent(out Destroyable destroyable))
                    {
                        destroyable.TakeDamage(Damage, _transform);
                    }
                }

                _attack = false;
            }
        }
        else
        {
            _swingTimeoutDelta = SwingTimeout;

            if (_attackTimeoutDelta >= 0)
            {
                _attackTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _anim.SetBool(_animIDAttack, false);
            }
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
                Jump();
                LookTarget();
                Attack();
                SearchCollectables();
                break;
            case MoveType.Fly:
                Fly();
                CharacterFlyRotation();
                SearchCollectables();
                if (_isGround)
                {
                    MoveStateChange(MoveType.Walk);
                }
                break;
            default:
                break;
        }
    }
    private void Fly()
    {
        _rb.velocity = (CharacterTransform.forward + _targetDirection) * _currentMoveSettings.Speed * _smoothMoveVector.magnitude;

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
        camRoot.transform.rotation = _lookDirection;
    }
    private void CharacterWalkRotation()
    {
        CharacterTransform.rotation = SlerpYAxisRotation();
    }
    private void CharacterFlyRotation()
    {
        CharacterTransform.rotation = SlerpLookDirection();
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
    public void VirtualAttackInput()
    {
        if (_attackCooldownDelta > 0)
        {
            return;
        }

        _attackCooldownDelta = Cooldown;
        _attack = true;
    }
    public void MoveStateChange(MoveType state)
    {
        if (!state.Equals(_moveType))
        {
            _prevMoveType = _moveType;
        }

        switch (state)
        {
            case MoveType.Walk:
                StartWalking();
                break;
            case MoveType.Fly:
                StartCoroutine(StartFlying());
                break;
            default:
                break;
        }
    }
    public void SetPreviusMoveState()
    {
        switch (_prevMoveType)
        {
            case MoveType.Walk:
                StartWalking();
                break;
            case MoveType.Fly:
                StartCoroutine(StartFlying());
                break;
            default:
                break;
        }
    }
    private void StartWalking()
    {
        ChangeCam(cam_Walk);

        _anim.SetBool(_animIDFly, false);
        _inputState.SetState(InputState.MainWalk);
        _moveType = MoveType.Walk;
        _currentMoveSettings = GetSettings(_moveType);
    }
    private IEnumerator StartFlying()
    {
        ChangeCam(cam_Fly);
        _jump = true;

        yield return new WaitForSeconds(_currentMoveSettings.JumpTimeout);

        _anim.SetBool(_animIDFly, true);
        _inputState.SetState(InputState.MainFly);
        _moveType = MoveType.Fly;
        _jump = false;
        _currentMoveSettings = GetSettings(_moveType);
    }
    #endregion


    #region Utility
    private void ChangeCam(GameObject camToActivate)
    {
        cam_Walk.SetActive(false);
        cam_Fly.SetActive(false);

        camToActivate.SetActive(true);
    }
    private void GroundedCheck()
    {
        Vector3 spherePosition = _transform.position;
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
    private float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle, 360);
        min = Mathf.Repeat(min, 360);
        max = Mathf.Repeat(max, 360);
        bool inverse = false;
        var tmin = min;
        var tangle = angle;
        if (min > 180)
        {
            inverse = !inverse;
            tmin -= 180;
        }
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        var result = !inverse ? tangle > tmin : tangle < tmin;
        if (!result)
            angle = min;

        inverse = false;
        tangle = angle;
        var tmax = max;
        if (angle > 180)
        {
            inverse = !inverse;
            tangle -= 180;
        }
        if (max > 180)
        {
            inverse = !inverse;
            tmax -= 180;
        }

        result = !inverse ? tangle < tmax : tangle > tmax;
        if (!result)
            angle = max;
        return angle;
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
        Transform camTargetT = camRoot;

        float XaxisRotation = _look.x;
        float YaxisRotation = _look.y;

        float botClampLim = _currentMoveSettings.BottomClamp;
        float topClampLim = _currentMoveSettings.TopClamp;

        float yRotation = Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.y,
            camTargetT.eulerAngles.y + XaxisRotation * 10f,
            ref _rotationVelocityY, _currentMoveSettings.CameraSmoothTimer);
        float xRotation = Mathf.SmoothDampAngle(
            camTargetT.eulerAngles.x,
            camTargetT.eulerAngles.x - YaxisRotation * 10f,
            ref _rotationVelocityX, _currentMoveSettings.CameraSmoothTimer);

        xRotation = ClampAngle(xRotation, botClampLim, topClampLim);

        return Quaternion.Euler(xRotation, yRotation, 0.0f);
    }
    private Quaternion SlerpLookDirection()
    {
        return Quaternion.Slerp(
            CharacterTransform.rotation,
            CalculateLookDirection(),
            _currentMoveSettings.CharacterRotationSpeed * Time.deltaTime);
    }
    private Quaternion SlerpYAxisRotation()
    {
        Transform camTargetT = camRoot;

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
    public MoveType GetMoveType() => _moveType;
    public Transform LookTarget()
    {
        Vector3 rayOrigin = new Vector3(0.5f, 0.5f, 0f);
        RaycastHit hit;
        Ray ray = _cam.ViewportPointToRay(rayOrigin);

        if (Physics.Raycast(ray, out hit, TargetLength, TargetLayer))
        {
            Transform objectHit = hit.transform;
            Debug.DrawRay(_cam.transform.position, _cam.transform.forward * TargetLength, Color.green);
            return objectHit;
        }
        else
        {
            Debug.DrawRay(_cam.transform.position, _cam.transform.forward * TargetLength, Color.red);
            return null;
        }
    }
    public void SearchCollectables()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_transform.position, TakeCollectablesRadius, CollectableLayer);

        foreach (Collider coll in hitColliders)
        {
            if(coll.TryGetComponent(out Collectable collectable))
            {
                collectable.Take(_transform);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, TakeCollectablesRadius);
    }
    #endregion
}
