using UnityEngine;

public class WarriorBehaviourMachine : FSM
{
    Vector3 moveDirDamp;

    protected override void Update()
    {
        base.Update();

        Debug.DrawRay(transform.Find("CharacterTarget").position, moveDirection * moveDirection.magnitude);

        ValueY = Mathf.Lerp(ValueY, Vector3.Dot(moveDirection.normalized, transform.forward.normalized), 0.1f);
        ValueX = Mathf.Lerp(ValueX, Vector3.Dot(moveDirection.normalized, transform.right.normalized), 0.1f);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        UpdateLocomotion();
    }
    protected override void InitializeStates()
    {
        behaviours.Add(CharacterBehaviour.Idling, new EnemyIdling(this));
        behaviours.Add(CharacterBehaviour.Running, new EnemyStandingoff(this));
        behaviours.Add(CharacterBehaviour.Sprinting, new EnemyChasing(this));
        behaviours.Add(CharacterBehaviour.Stoping, new EnemyStoping(this));

        behaviours.Add(CharacterBehaviour.Falling, new EnemyFalling(this));
        behaviours.Add(CharacterBehaviour.Landing, new EnemyLanding(this));

        behaviours.Add(CharacterBehaviour.LandAttacking, new EnemyLandAttacking(this));
        behaviours.Add(CharacterBehaviour.Dodging, new EnemyDodging(this));

        behaviours.Add(CharacterBehaviour.Suffering, new EnemySuffering(this));

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
        moveDirDamp = Vector3.Lerp(moveDirDamp, moveDirection, 0.1f);
        Rigidbody.MovePosition(Rigidbody.position + verticalVelocity * Vector3.up * Time.fixedDeltaTime);
        Rigidbody.MovePosition(Rigidbody.position + currentMoveSpeed * moveDirDamp * Time.fixedDeltaTime);
        transform.rotation = rotateDirection;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GroundPoint, 0.1f);
        Gizmos.DrawRay(ColliderUtility.CapsuleCollider.bounds.center, Vector3.down * ColliderUtility.FloatRayDistance);

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(ColliderUtility.CapsuleCollider.bounds.center, Vector3.down * GroundCheckSetting.groundCheckDistance);
        Gizmos.DrawCube(transform.position + GroundCheckSetting.cubeCheckOffset, GroundCheckSetting.groundCheckExtents);
    }
}