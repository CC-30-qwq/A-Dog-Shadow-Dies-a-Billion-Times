using UnityEngine;

public class PlayerUsingSkill_0 : BaseBehaviour
{
    Vector3 AttackDirection;
    bool attackRotate;
    public PlayerUsingSkill_0(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        AttackDirection = bm.transform.forward;

        attackRotate = true;

        bm.Effects.Trails.Begin();

        bm.Animator.CrossFade(CachedAnimator.SkillAttack_0, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        bm.BufferDirection(false);

        if (bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0)
        {
            attackRotate = false;
        }

        if (attackRotate)
        {
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

        if (bm.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0;
        }

        if (behaviourTimer > bm.Status.Skill.SkillDuration)
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

        if (behaviourTimer > bm.Status.ActionCooldown * 3)
        {
            bm.UpdateMoveDirection(bm.transform.forward);
        }
        else
        {
            bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        }

        bm.UpdateRotate(bm.Status.MediumRotateSpeed, AttackDirection);

        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        if (bm.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = false;
        }

        bm.Effects.Trails.End();
    }
}

public class PlayerUsingSkill_1 : BaseBehaviour
{
    Vector3 AttackDirection;
    bool attackRotate;
    public PlayerUsingSkill_1(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        AttackDirection = bm.transform.forward;

        attackRotate = true;

        bm.Effects.Trails.Begin();

        bm.Animator.CrossFade(CachedAnimator.SkillAttack_1, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        bm.BufferDirection(false);

        if (bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0)
        {
            attackRotate = false;
        }

        if (attackRotate)
        {
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

        if (bm.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = bm.Animator.GetFloat(CachedAnimator.OnAttack) != 0;
        }

        if (behaviourTimer > bm.Status.Skill.SkillDuration)
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

        if (behaviourTimer > bm.Status.ActionCooldown * 2)
        {
            bm.UpdateMoveDirection(bm.transform.forward);
        }
        else
        {
            bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        }

        bm.UpdateRotate(bm.Status.MediumRotateSpeed, AttackDirection);

        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.Animator.SetFloat("Move", 0);

        if (bm.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = false;
        }

        bm.Effects.Trails.End();
    }
}