using System.Collections;
using UnityEngine;

#region Base
public class ArcherGround : BaseBehaviour
{
    public ArcherGround(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (!FSM.isGrounded)
        {
            FSM.TryChangeState(CharacterBehaviour.Falling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.Float();
    }
}
#endregion

#region Ground
public class ArcherIdle : EnemyGround
{
    // 缓存Animator Hash值避免重复字符串哈希计算
    int IdleHash = Animator.StringToHash("Idle");

    public ArcherIdle(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.Animator.CrossFade(IdleHash, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // 简化条件判断
        if (FSM.AI.CurrentTarget != null && FSM.AI.TargetDistance < FSM.AI.HardDistance)
        {
            FSM.TryChangeState(CharacterBehaviour.Running);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveSpeed(0, FSM.Status.HardMoveDrag);
    }
}

public class ArcherStandingoff : EnemyGround
{
    // 使用常量替换魔法数字
    float MinCircleTime = 2f;
    float MaxCircleTime = 4f;
    int MaxDirectionOptions = 5;
    int MaxAttackBack = 3;

    float moveDirection;
    float circleTime;
    int attackBack;

    public ArcherStandingoff(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        circleTime = Random.Range(MinCircleTime, MaxCircleTime);
        moveDirection = Random.Range(0, MaxDirectionOptions);
        attackBack = Random.Range(0, MaxAttackBack);

        FSM.Animator.CrossFade("Move", 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (FSM.AI.Move)
        {
            FSM.TryChangeState(CharacterBehaviour.Sprinting);
        }

        // 简化攻击条件判断
        if (attackBack == 0 || behaviourTimer > circleTime)
        {
            FSM.TryChangeState(CharacterBehaviour.LandAttacking);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveSpeed(FSM.Status.LightMoveSpeed, FSM.Status.MediumMoveDrag);
        FSM.UpdateRotate(FSM.Status.HardRotateSpeed, FSM.targetDirection);
        FSM.UpdateMoveDirection(GetRandomDirection());
    }

    private Vector3 GetRandomDirection()
    {
        return moveDirection switch
        {
            0 or 1 => FSM.transform.right.normalized,
            2 or 3 => -FSM.transform.right.normalized,
            4 => -FSM.transform.forward.normalized,
            _ => Vector3.zero
        };
    }
}

public class ArcherChasing : EnemyGround
{
    private const float MaxChaseTime = 5f;

    public ArcherChasing(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.Animator.CrossFade("Move", 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (FSM.AI.Standoff && behaviourTimer > 0.5f)
        {
            FSM.TryChangeState(CharacterBehaviour.Running);
        }

        if (behaviourTimer > MaxChaseTime)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateAgentPosition(FSM.AI.CurrentTarget.position);

        var direction = (FSM.AI.Agent.steeringTarget - FSM.transform.position).normalized;
        FSM.UpdateMoveDirection(direction);

        if (FSM.AI.Move)
        {
            FSM.UpdateMoveSpeed(FSM.Status.MediumMoveSpeed, FSM.Status.MediumMoveDrag);
        }
        else if (FSM.AI.Attack)
        {
            FSM.UpdateMoveSpeed(0, FSM.Status.MediumMoveDrag);
        }

        FSM.UpdateRotate(FSM.Status.HardRotateSpeed, FSM.moveDirection);
    }
}
#endregion

#region Combat
public class ArcherLandAttacking : BaseBehaviour
{
    // 缓存Animator参数
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    bool attackRotate;

    private int attackCount;
    private Vector3 attackDirection;
    private bool hasAttacked;
    private bool canCombo;
    private int currentCombo = 0;
    private float attackDuration;

    public ArcherLandAttacking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        attackCount = FSM.AI.AttackCount[0];
        FSM.verticalVelocity = 0;
        attackDirection = FSM.transform.forward;
        hasAttacked = false;
        canCombo = false;
        currentCombo = 0;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (!FSM.isGrounded)
        {
            FSM.TryChangeState(CharacterBehaviour.Falling);
        }

        if (!hasAttacked)
        {
            attackRotate = true;

            StartStateCoroutine(EnableComboWindow());
            attackDuration = FSM.Status.NormalCombo[currentCombo].attackDurations.NormalAttackDuration;
        }
        else
        {
            if (FSM.Animator.GetFloat(CachedAnimator.OnAttack) != 0)
            {
                attackRotate = false;
            }

            if (attackRotate)
            {
                attackDirection = FSM.targetDirection;
            }
        }

        if (canCombo && FSM.AI.Attack && attackCount > 0)
        {
            hasAttacked = false;
        }

        if (behaviourTimer > attackDuration + FSM.Status.NormalCombo[currentCombo].ComboWindow)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateAgentPosition(FSM.transform.position);
        bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        bm.UpdateRotate(bm.Status.HardRotateSpeed, attackDirection);
        bm.UpdateMoveDirection(bm.transform.forward);
        bm.Float();
    }

    private IEnumerator EnableComboWindow()
    {
        behaviourTimer = 0;
        hasAttacked = true;
        canCombo = false;
        FSM.Animator.CrossFade("Attack_" + currentCombo, 0.2f, 1);
        attackCount -= 1;
        yield return new WaitForSeconds(FSM.Status.NormalCombo[currentCombo].attackDurations.NormalAttackDuration);
        currentCombo = FSM.Status.NormalCombo[currentCombo].NextCombo;
        canCombo = true;
    }
}

public class ArcherDodging : BaseBehaviour
{
    // 使用缓存的Animator参数
    public ArcherDodging(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.Animator.CrossFade(CachedAnimator.Avoid, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        if (behaviourTimer > bm.Status.Dodge.DodgeDuration)
        {
            bm.TryChangeState(CharacterBehaviour.Stoping);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveSpeed(bm.Status.Dodge.DodgeSpeed, bm.Status.HardMoveDrag);
        bm.UpdateMoveDirection(-FSM.transform.forward);
        bm.UpdateRotate(bm.Status.MediumRotateSpeed, bm.targetDirection);
        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        FSM.currentMoveSpeed = FSM.Status.MediumMoveSpeed;
    }
}
#endregion

#region Hit
public class ArcherSuffering : BaseBehaviour
{
    private const int HitDamage = 5;

    public ArcherSuffering(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;
        if (bm == null) return;

        bm.verticalVelocity = 0;

        if (bm.Health != null)
        {
            bm.Health.HP -= HitDamage;
        }

        var animKey = GetHitAnimKey(bm.hitAngle);

        if (bm.Animator != null)
        {
            var hash = CombosHashCache.Get(animKey);
            bm.Animator.CrossFade(hash, 0.2f, 1);
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (behaviourTimer > FSM.Status.Damage.HitDuration)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        FSM.UpdateAgentPosition(FSM.transform.position);
        bm.UpdateMoveSpeed(bm.Status.Damage.moveSpeeds[0].moveSpeed, bm.Status.HardMoveDrag);
        bm.UpdateMoveDirection(-bm.DamageDirection);
        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        FSM.hasDamaged = false;

        if (FSM.Animator != null)
        {
            FSM.Animator.CrossFade(CachedAnimator.Idle, 0.2f, 1);
        }

        FSM.currentMoveSpeed = 0;
    }

    private static string GetHitAnimKey(float angle)
    {
        return angle switch
        {
            <= 30 and >= -30 => "Hit_F",
            > 30 and <= 90 => "Hit_FR",
            > 90 and <= 150 => "Hit_R",
            < -30 and >= -90 => "Hit_FL",
            < -90 and >= -150 => "Hit_L",
            > 150 or < -150 => "Hit_B",
            _ => "Hit_F"
        };
    }
}
#endregion
