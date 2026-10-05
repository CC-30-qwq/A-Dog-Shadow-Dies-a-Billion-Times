using UnityEngine;

public class PlayerBehaviourMachine : FSM
{
    [field: SerializeField] public Camera PlayerCamera { get; set; }

    protected override void Update()
    {
        base.Update();

        Debug.DrawRay(transform.Find("CharacterTarget").position, moveDirection * moveDirection.magnitude);

        ValueY = Mathf.Lerp(ValueY, Vector3.Dot(moveDirection.normalized, transform.forward.normalized), 0.1f);
        ValueX = Mathf.Lerp(ValueX, Vector3.Dot(moveDirection.normalized, transform.right.normalized), 0.1f);

        UpdateInputDirection();
        UpdateCameraDirection();

        PlayerSkillChosing();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateLocomotion();
    }
    protected override void InitializeStates()
    {
        // Movement States
        behaviours.Add(CharacterBehaviour.Idling, new PlayerIdle(this));
        behaviours.Add(CharacterBehaviour.Starting, new PlayerStarting(this));
        behaviours.Add(CharacterBehaviour.Running, new PlayerRunning(this));
        behaviours.Add(CharacterBehaviour.Dodging, new PlayerDodging(this));
        behaviours.Add(CharacterBehaviour.Sprinting, new PlayerSprinting(this));
        behaviours.Add(CharacterBehaviour.Stoping, new PlayerStoping(this));

        // Airborne States
        behaviours.Add(CharacterBehaviour.Jumping, new PlayerJumping(this));
        behaviours.Add(CharacterBehaviour.Falling, new PlayerFalling(this));
        behaviours.Add(CharacterBehaviour.Landing, new PlayerLanding(this));

        // Combat States
        behaviours.Add(CharacterBehaviour.LandAttacking, new PlayerLandAttacking(this));
        behaviours.Add(CharacterBehaviour.SprintAttacking, new PlayerSprintAttacking(this));
        behaviours.Add(CharacterBehaviour.AirAttacking, new PlayerAirAttacking(this));
        behaviours.Add(CharacterBehaviour.Blocking, new PlayerBlocking(this));
        behaviours.Add(CharacterBehaviour.Defencing, new PlayerDefencing(this));

        // Damage States
        behaviours.Add(CharacterBehaviour.Suffering, new PlayerSuffering(this));
        behaviours.Add(CharacterBehaviour.Parring, new PlayerParring(this));
        behaviours.Add(CharacterBehaviour.Hiting, new PlayerHiting(this));

        // Skill State
        behaviours.Add(CharacterBehaviour.UsingSkill_0, new PlayerUsingSkill_0(this));
        behaviours.Add(CharacterBehaviour.UsingSkill_1, new PlayerUsingSkill_1(this));

        currentBehaviour = behaviours[CharacterBehaviour.Idling];
        currentBehaviourType = CharacterBehaviour.Idling;
        currentBehaviour.EnterState();
    }

    public override void UpdateAnimator()
    {
        Animator.SetFloat("State", currentBehaviourType.GetHashCode());

        Animator.SetFloat("MoveSpeed_Y", ValueY * currentMoveSpeed / Status.HardMoveSpeed);
        Animator.SetFloat("MoveSpeed_X", ValueX * currentMoveSpeed / Status.HardMoveSpeed);
    }

    public override void UpdateLocomotion()
    {
        Rigidbody.MovePosition(Rigidbody.position + verticalVelocity * Vector3.up * Time.fixedDeltaTime);
        Rigidbody.MovePosition(Rigidbody.position + currentMoveSpeed * moveDirection * Time.fixedDeltaTime);
        Rigidbody.MoveRotation(rotateDirection);
        transform.rotation = rotateDirection;
    }

    #region 输入方法
    public override void BufferDirection(bool CanBuffer)
    {
        if (CanBuffer)
        {
            if (Input.MoveIns.sqrMagnitude > 0.9f)
            {
                BufferedDirection = Quaternion.LookRotation(CameraDirection) * new Vector3(Input.MoveIns.x, 0, Input.MoveIns.y).normalized;
            }
        }
        else
        {
            BufferedDirection = transform.forward;
        }
    }
    public void UpdateInputDirection()
    {
        InputDirection = Quaternion.LookRotation(CameraDirection) * new Vector3(Input.MoveIns.x, 0, Input.MoveIns.y).normalized;
    }
    public void UpdateCameraDirection()
    {
        if (PlayerCamera != null)
        {
            Vector3 cameraForward = PlayerCamera.transform.forward;
            cameraForward.y = 0;
            CameraDirection = cameraForward;
        }
    }
    #endregion

    public override void PlayerSkillChosing()
    {
        if (Input.Skill_0Ins)
        {
            Status.Skill.currentSkillHash = 0;
        }
        else if (Input.Skill_1Ins)
        {
            Status.Skill.currentSkillHash = 1;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GroundPoint, 0.1f);
        Gizmos.DrawRay(ColliderUtility.CapsuleCollider.bounds.center, Vector3.down * ColliderUtility.FloatRayDistance);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(ColliderUtility.CapsuleCollider.bounds.center, Vector3.down * GroundCheckSetting.groundCheckDistance);
        Gizmos.DrawCube(transform.position + GroundCheckSetting.cubeCheckOffset, GroundCheckSetting.groundCheckExtents);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * CombatSetting.attackDection.x + transform.up * CombatSetting.attackDection.y, CombatSetting.attackDection.z);
    }
}