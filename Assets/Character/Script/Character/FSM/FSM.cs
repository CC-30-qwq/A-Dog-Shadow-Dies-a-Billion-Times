using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CharacterBehaviour
{
    Idling,
    Starting,
    Running,
    Dodging,
    Sprinting,
    Stoping,

    Jumping,
    Falling,
    Landing,

    LandAttacking,
    SprintAttacking,
    AirAttacking,
    Blocking,
    Defencing,

    Suffering,
    Parring,
    Hiting,

    UsingSkill_0,
    UsingSkill_1,
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BehaviourStatus))]
[RequireComponent(typeof(HealthSystem))]
public abstract class FSM : MonoBehaviour
{
    protected Dictionary<CharacterBehaviour, BaseBehaviour> behaviours;
    protected BaseBehaviour currentBehaviour;

    public Animator Animator { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public BehaviourStatus Status { get; private set; }
    public HealthSystem Health { get; private set; }
    [field: SerializeField] public CapsuleColliderUtility ColliderUtility { get; set; }
    [field: SerializeField] public LayerMask GroundLayer { get; set; }

    #region Variable
    [Header("Current Status")]
    public CharacterBehaviour currentBehaviourType;

    public bool lockMove;
    public Vector3 targetDirection;

    public Vector3 moveDirection;
    public Quaternion rotateDirection;
    public float currentMoveSpeed;

    public float verticalVelocity;
    public float JumpDistance;

    public bool hasDamaged = false;
    public Vector3 DamagePosition;
    public Vector3 DamageDirection;
    public float hitAngle;

    [Header("Animation")]
    public float ValueY;
    public float ValueX;

    [Header("GroundCheck")]
    public GroundCheck GroundCheckSetting;
    public Collider[] groundCheckColliders;
    public bool isGrounded;
    public Vector3 GroundPoint;
    public bool groundRayHitValid;

    [Header("Float")]
    public Float FloatSetting;
    public float floatOffset;

    [Header("Combat")]
    public Combat CombatSetting;

    [Header("FootIK")]
    public Transform leftFoot;
    public Transform rightFoot;
    public RaycastHit groundRayHit;

    [Header("Effect")]
    public Effect Effects;
    #endregion

    [Header("Debug")]
    public Text debugText;

    #region 状态机方法
    public CharacterBehaviour CurrentBehaviour => currentBehaviourType;

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody>();
        ColliderUtility.Initialize(gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();

        Status = GetComponent<BehaviourStatus>();
        Health = GetComponent<HealthSystem>();

        Input = GetComponent<PlayerInputs>();
        AI = GetComponent<CharacterAI>();

        if (Animator != null)
        {
            leftFoot = Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            rightFoot = Animator.GetBoneTransform(HumanBodyBones.RightFoot);
        }
    }

    protected virtual void Start()
    {
        behaviours = new Dictionary<CharacterBehaviour, BaseBehaviour>();
        InitializeStates();

        rotateDirection = transform.rotation;
        moveDirection = transform.forward;

        if (CombatSetting.WeaponCollider != null)
        {
            CombatSetting.WeaponCollider.enabled = false;
        }
    }

    protected virtual void OnValidate()
    {
        ColliderUtility.Initialize(gameObject);
        ColliderUtility.CalculateCapsuleColliderDimensions();
    }

    protected virtual void Update()
    {
        currentBehaviour?.UpdateState();

        UpdateAnimator();
        UpdateGroundStatus();
        UpdateFloat();
    }

    protected virtual void FixedUpdate()
    {
        currentBehaviour?.FixedUpdateState();
    }

    protected abstract void InitializeStates();

    public virtual void TryChangeState(CharacterBehaviour newState)
    {
        currentBehaviour?.ExitState();

        if (behaviours.ContainsKey(newState))
        {
            currentBehaviour = behaviours[newState];
            currentBehaviourType = newState;
            currentBehaviour.EnterState();
        }
    }
    #endregion

    #region 基础方法
    public void Float()
    {
        Vector3 capsuleCenter = ColliderUtility.CapsuleCollider.bounds.center;

        if (Physics.Raycast(capsuleCenter, Vector3.down, out RaycastHit hit, ColliderUtility.FloatRayDistance, GroundLayer))
        {
            float distanceToFloatingPoint = ColliderUtility.ColliderCenterInLocalSpace.y * transform.localScale.y - hit.distance;

            if (distanceToFloatingPoint == 0f)
            {
                return;
            }

            Rigidbody.AddForce(new Vector3(0f, distanceToFloatingPoint * ColliderUtility.StepReachForce - Rigidbody.velocity.y + floatOffset, 0f), ForceMode.VelocityChange);
        }
    }
    public void UpdateFloat()
    {
        if (isGrounded)
        {
            float leftDiffer = leftFoot != null ? FootIKDiffer(leftFoot) : 0f;
            float rightDiffer = rightFoot != null ? FootIKDiffer(rightFoot) : 0f;

            float differ = Mathf.Max(leftDiffer, rightDiffer) * FloatSetting.floatMutiplier;
            if (floatOffset <= 0)
            {
                floatOffset = Mathf.Lerp(floatOffset, -differ, FloatSetting.damp);
            }
            else
            {
                floatOffset = 0;
            }
        }
        else
        {
            floatOffset = 0;
        }
    }
    public float FootIKDiffer(Transform foot)
    {
        if (foot == null)
            return 0f;

        Vector3 raycastOrigin = new Vector3(foot.position.x, transform.position.y + FloatSetting.rayHight, foot.position.z);

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, 2 * FloatSetting.rayHight, GroundLayer))
        {
            float differ = raycastOrigin.y - hit.point.y - FloatSetting.rayHight;
            return differ;
        }
        else
        {
            return 0f;
        }
    }
    private void UpdateGroundStatus()
    {
        groundRayHitValid = Physics.Raycast(ColliderUtility.CapsuleCollider.bounds.center, Vector3.down, out groundRayHit, GroundCheckSetting.groundCheckDistance, GroundLayer);

        groundCheckColliders = Physics.OverlapBox(transform.position + GroundCheckSetting.cubeCheckOffset, GroundCheckSetting.groundCheckExtents, Quaternion.identity, GroundLayer);

        isGrounded = groundRayHitValid || groundCheckColliders.Length > 0;

        if (isGrounded)
        {
            if (groundRayHitValid)
            {
                GroundPoint = groundRayHit.point;
            }
            else
            {
                GroundPoint = transform.position;
            }
        }
    }
    public abstract void UpdateAnimator();
    public virtual void UpdateLocomotion() { }
    public void UpdateMoveDirection(Vector3 Direction)
    {
        moveDirection = Direction.normalized;
        moveDirection.y = 0;
    }
    public void TransformPosition(float MoveSpeed)
    {
        Rigidbody.MovePosition(Rigidbody.position + MoveSpeed * InputDirection * Time.fixedDeltaTime);
    }
    public void UpdateMoveSpeed(float TargetSpeed, float DragForce)
    {
        if (TargetSpeed != 0)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, TargetSpeed, DragForce * Time.fixedDeltaTime);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, TargetSpeed, DragForce * Time.fixedDeltaTime);
            if (currentMoveSpeed < 0.1)
            {
                currentMoveSpeed = 0;
            }
        }
    }
    public void UpdateRotate(float RotateSpeed, Vector3 TargetDirection)
    {
        rotateDirection = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(TargetDirection), RotateSpeed * Time.fixedDeltaTime);
    }
    public void Fall(float GravityMultiplier)
    {
        verticalVelocity = verticalVelocity > Physics.gravity.y ? verticalVelocity + Physics.gravity.y * GravityMultiplier * Time.fixedDeltaTime : Physics.gravity.y;
    }
    public bool DetectAttackTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward * CombatSetting.attackDection.x + transform.up * CombatSetting.attackDection.y, CombatSetting.attackDection.z, CombatSetting.targetMask);
        if (targets.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region 玩家
    public PlayerInputs Input { get; private set; }
    public Vector3 CameraDirection { get; set; }
    public Vector3 InputDirection { get; set; }
    public Vector3 BufferedDirection { get; set; }

    public virtual void BufferDirection(bool CanBuffer) { }
    public virtual void PlayerSkillChosing() { }
    #endregion

    #region AI
    public CharacterAI AI { get; private set; }

    public virtual void UpdateAgentPosition(Vector3 TargetPosition)
    {
        AI.Agent.speed = currentMoveSpeed;
        AI.Agent.SetDestination(TargetPosition);
    }
    #endregion
}
