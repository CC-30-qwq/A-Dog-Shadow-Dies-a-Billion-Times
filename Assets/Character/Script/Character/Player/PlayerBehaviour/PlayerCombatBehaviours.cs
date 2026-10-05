using System.Collections;
using UnityEngine;

public class PlayerAttack : BaseBehaviour
{
    public PlayerAttack(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Effects.Trails.Begin();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        bm.BufferDirection(false);


        if (bm.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0;
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.floatOffset = 0;

        if (bm.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = false;
        }

        bm.Effects.Trails.End();
    }
}
public class PlayerDefence : BaseBehaviour
{
    public PlayerDefence(FSM machine) : base(machine) { }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        if (bm.Input.AttackIns && behaviourTimer < 0.03f)
        {
            if (bm.Status.Skill.currentSkillHash == 0)
            {
                bm.TryChangeState(CharacterBehaviour.UsingSkill_0);
            }
            else if (bm.Status.Skill.currentSkillHash == 1)
            {
                bm.TryChangeState(CharacterBehaviour.UsingSkill_1);
            }
        }
    }
}

public class PlayerLandAttacking : PlayerAttack
{
    Vector3 AttackDirection;
    bool hasCharged;
    bool hasAttacked;
    bool canCombo;
    int currentCombo = 0;
    float RotateSpeed;
    float animOffset;
    float chargeTime;
    float attackDuration;
    bool attackRotate;

    public PlayerLandAttacking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        AttackDirection = FSM.transform.forward;
        hasCharged = false;
        hasAttacked = false;
        canCombo = false;
        currentCombo = 0;
        chargeTime = 0;
        animOffset = 0;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        if (bm.Input.DefenceIns && behaviourTimer < bm.Status.AttackCancelTime)
        {
            bm.Input.AttackIns = false;
            if (behaviourTimer > bm.Status.AttackCancelTime - bm.Status.ActionCooldown / 2)
            {
                bm.TryChangeState(CharacterBehaviour.Blocking);
            }
        }

        if (!hasAttacked)
        {
            attackRotate = true;

            if (bm.Input.ChargeIns && bm.Status.NormalCombo[currentCombo].CanCharge)
            {
                behaviourTimer = 0;

                chargeTime += Time.deltaTime;

                if (!hasCharged)
                {
                    chargeTime = 0;
                    hasCharged = true;
                    RotateSpeed = bm.Status.NormalCombo[currentCombo].rotateSpeeds.ChargeAttackRotateSpeed;
                    var typ = bm.Status.NormalCombo[currentCombo].ChargeAttackType;
                    bm.Animator.CrossFade(CombosHashCache.Get("Charge_" + typ), 0.2f, 1, animOffset);
                }

                if (!bm.lockMove)
                {
                    if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
                    {
                        AttackDirection = bm.InputDirection;
                    }
                }
                else
                {
                    AttackDirection = bm.targetDirection;
                }
            }
            else if (chargeTime < bm.Status.ChargeTime)
            {
                chargeTime = 0;
                animOffset = 0.5f;
                StartStateCoroutine(EnableNormalComboWindow());
                attackDuration = bm.Status.NormalCombo[currentCombo].attackDurations.NormalAttackDuration;
            }

            if (chargeTime >= bm.Status.ChargeTime)
            {
                chargeTime = 0;
                animOffset = 0.5f;
                StartStateCoroutine(EnableChargeComboWindow());
                attackDuration = bm.Status.NormalCombo[currentCombo].attackDurations.ChargeAttackDuration;
            }
        }
        else
        {
            if (!bm.lockMove)
            {
                if (behaviourTimer < bm.Status.AttackGetRotateTime)
                {
                    if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
                    {
                        AttackDirection = bm.InputDirection;
                    }
                }
            }
            else
            {
                if (bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0)
                {
                    attackRotate = false;
                }

                if (attackRotate)
                {
                    AttackDirection = bm.targetDirection;
                }
            }
        }

        if (canCombo && bm.Input.AttackIns)
        {
            hasAttacked = false;
            hasCharged = false;
        }

        if (behaviourTimer > attackDuration)
        {
            if (bm.Input.DefenceIns)
            {
                bm.TryChangeState(CharacterBehaviour.Defencing);
            }

            if (bm.Input.DodgeIns)
            {
                bm.TryChangeState(CharacterBehaviour.Dodging);
            }
        }

        if (behaviourTimer > attackDuration + bm.Status.NormalCombo[currentCombo].ComboWindow)
        {
            bm.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;
        var animMove = bm.Animator.GetFloat(CachedAnimator.Move);

        if (!bm.DetectAttackTarget())
        {
            bm.UpdateMoveSpeed(animMove * bm.Status.HardMoveSpeed, bm.Status.HardMoveDrag);
        }
        else
        {
            bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        }

        if (hasAttacked)
        {
            bm.UpdateMoveDirection(bm.transform.forward);
        }

        bm.UpdateRotate(RotateSpeed, AttackDirection);

        bm.Float();
    }

    private IEnumerator EnableChargeComboWindow()
    {
        behaviourTimer = 0;
        hasAttacked = true;
        canCombo = false;
        RotateSpeed = FSM.Status.NormalCombo[currentCombo].rotateSpeeds.ChargeAttackRotateSpeed;
        var typ = FSM.Status.NormalCombo[currentCombo].ChargeAttackType;
        FSM.Animator.CrossFade(CombosHashCache.Get("ChargeAttack_" + typ), 0.1f, 1);
        yield return new WaitForSeconds(FSM.Status.NormalCombo[currentCombo].attackDurations.ChargeAttackDuration);
        canCombo = true;
    }
    private IEnumerator EnableNormalComboWindow()
    {
        behaviourTimer = 0;
        hasAttacked = true;
        canCombo = false;
        RotateSpeed = FSM.Status.NormalCombo[currentCombo].rotateSpeeds.NormalAttackRotateSpeed;
        FSM.Animator.CrossFade(CombosHashCache.Get("Attack_" + currentCombo), 0.1f, 1);
        yield return new WaitForSeconds(FSM.Status.NormalCombo[currentCombo].attackDurations.NormalAttackDuration);
        currentCombo = FSM.Status.NormalCombo[currentCombo].NextCombo;
        canCombo = true;
    }
}

public class PlayerSprintAttacking : PlayerAttack
{
    bool hasAttacked;
    Vector3 AttackDirection;
    bool attackRotate;

    public PlayerSprintAttacking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.currentMoveSpeed = 0;

        hasAttacked = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!hasAttacked)
        {
            attackRotate = true;
            bm.Animator.CrossFade(CachedAnimator.SprintAttack, 0.2f, 1);
            hasAttacked = true;
        }
        else
        {
            if (!bm.lockMove)
            {
                if (behaviourTimer < bm.Status.AttackGetRotateTime)
                {
                    if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
                    {
                        AttackDirection = bm.InputDirection;
                    }
                }
            }
            else
            {
                if (bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0)
                {
                    attackRotate = false;
                }

                if (attackRotate)
                {
                    AttackDirection = bm.targetDirection;
                }
            }
        }

        if (behaviourTimer > bm.Status.SprintCombo.SprintAttackDuration)
        {
            bm.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;
        var animMove = bm.Animator.GetFloat(CachedAnimator.Move);

        if (!bm.DetectAttackTarget())
        {
            bm.UpdateMoveSpeed(animMove * bm.Status.HardMoveSpeed, bm.Status.HardMoveDrag);
        }
        else
        {
            bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        }

        bm.UpdateMoveDirection(bm.transform.forward);

        bm.UpdateRotate(bm.Status.MediumRotateSpeed, AttackDirection);

        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.currentMoveSpeed = 0;
    }
}

public class PlayerAirAttacking : PlayerAttack
{
    bool hasAttacked;
    bool canCombo;
    int currentCombo = 0;

    public PlayerAirAttacking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        hasAttacked = false;
        canCombo = false;
        currentCombo = 0;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!hasAttacked)
        {
            behaviourTimer = 0;
            StartStateCoroutine(EnableComboWindow());
            hasAttacked = true;
        }

        if (canCombo && bm.Input.AttackIns)
        {
            if (currentCombo < bm.Status.AirCombo.Count - 1)
            {
                currentCombo++;
            }
            else
            {
                currentCombo = 0;
            }
            hasAttacked = false;
        }
        else if (behaviourTimer > bm.Status.AirCombo[currentCombo].AttackDuration)
        {
            if (bm.isGrounded)
            {
                bm.TryChangeState(CharacterBehaviour.Landing);
            }
            else if (behaviourTimer > bm.Status.AirCombo[currentCombo].AttackDuration + bm.Status.AirCombo[currentCombo].ComboWindow)
            {
                bm.TryChangeState(CharacterBehaviour.Falling);
            }
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.Fall(bm.Status.GravityMultiplier);

        bm.UpdateMoveSpeed(0, bm.Status.LightMoveDrag);

        if (!bm.lockMove)
        {
            if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
            {
                bm.UpdateRotate(bm.Status.AirCombo[currentCombo].AttackRotateSpeed, bm.InputDirection);
            }
        }
        else
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
        }
        if (bm.isGrounded)
        {
            bm.verticalVelocity = 0;

            bm.Float();
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.JumpDistance = 0;
    }

    private IEnumerator EnableComboWindow()
    {
        canCombo = false;
        FSM.Animator.CrossFade(CombosHashCache.Get("AirAttack_" + currentCombo), 0.2f, 1);
        yield return new WaitForSeconds(FSM.Status.AirCombo[currentCombo].AttackDuration);
        if (!FSM.isGrounded)
        {
            canCombo = true;
        }
    }
}

public class PlayerBlocking : PlayerDefence
{
    public PlayerBlocking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Block_0, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        bm.TryChangeState(CharacterBehaviour.Defencing);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
        {
            bm.UpdateMoveDirection(bm.transform.forward);
        }
        bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.transform.forward);
        bm.Float();
    }
}

public class PlayerDefencing : PlayerDefence
{
    bool hasCanceled;
    bool canMove;

    public PlayerDefencing(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        canMove = true;
        hasCanceled = false;

        StartStateCoroutine(EnableBlockWindow());
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!hasCanceled && canMove)
        {
            behaviourTimer = 0;

            if (!bm.Input.DefenceIns)
            {
                hasCanceled = true;
                bm.Animator.CrossFade(CachedAnimator.Block_1, 0.2f, 1);
            }
        }

        if (hasCanceled)
        {
            canMove = false;
        }

        if (behaviourTimer > bm.Status.Block.BlockDuration)
        {
            bm.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        if (!bm.lockMove)
        {
            if (bm.Input.MoveIns.sqrMagnitude > 0.9f && canMove)
            {
                bm.UpdateMoveSpeed(bm.Status.LightMoveSpeed, bm.Status.HardMoveDrag);

                bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.InputDirection);
            }
            else
            {
                bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
            }

            if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
            {
                bm.UpdateMoveDirection(bm.transform.forward);
            }
        }
        else
        {
            if (bm.Input.MoveIns.sqrMagnitude > 0.9f && canMove)
            {
                bm.UpdateMoveSpeed(bm.Status.LightMoveSpeed, bm.Status.HardMoveDrag);
            }
            else
            {
                bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
            }

            if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
            {
                bm.UpdateMoveDirection(bm.InputDirection);
            }

            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
        }

        bm.Float();
    }

    public IEnumerator EnableBlockWindow()
    {
        canMove = false;
        yield return new WaitForSeconds(FSM.Status.ActionCooldown / 2);
        FSM.Animator.CrossFade(CachedAnimator.Defense, 0.1f, 1);
        canMove = true;
    }
}