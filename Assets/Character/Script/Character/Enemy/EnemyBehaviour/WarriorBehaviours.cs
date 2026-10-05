using System.Collections;
using UnityEngine;

#region Base
public class EnemyGround : BaseBehaviour
{
    public EnemyGround(FSM machine) : base(machine) { }

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
public class EnemyIdling : EnemyGround
{
    // 缓存Animator Hash值避免重复字符串哈希计算
    int IdleHash = Animator.StringToHash("Idle");

    public EnemyIdling(FSM machine) : base(machine) { }

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

public class EnemyStandingoff : EnemyGround
{
    // 使用常量替换魔法数字
    float MinCircleTime = 2f;
    float MaxCircleTime = 4f;
    float DirectionChangeThreshold = 1f;
    int MaxDirectionOptions = 5;
    int MaxAttackBack = 3;

    float moveDirection;
    float circleTime;
    int attackBack;

    public EnemyStandingoff(FSM machine) : base(machine) { }

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

        if (behaviourTimer > circleTime || behaviourTimer > DirectionChangeThreshold)
        {
            FSM.TryChangeState(CharacterBehaviour.Sprinting);
        }

        // 简化攻击条件判断
        if (FSM.AI.Attack && (attackBack == 0 || behaviourTimer > circleTime))
        {
            FSM.TryChangeState(CharacterBehaviour.LandAttacking);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveSpeed(FSM.Status.LightMoveSpeed, FSM.Status.HardMoveDrag);
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

public class EnemyChasing : EnemyGround
{
    int stand;
    private const float ChaseTime = 5f;

    public EnemyChasing(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.Animator.CrossFade("Move", 0.2f, 1);

        stand = Random.Range(0, 2);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (FSM.AI.Standoff && stand == 0 && behaviourTimer > 1)
        {
            FSM.TryChangeState(CharacterBehaviour.Running);
        }

        if (FSM.AI.Attack)
        {
            FSM.TryChangeState(CharacterBehaviour.LandAttacking);
        }

        if (behaviourTimer > ChaseTime)
        {
            FSM.TryChangeState(CharacterBehaviour.Stoping);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveDirection(FSM.targetDirection);

        if (FSM.AI.Move || FSM.AI.Standoff)
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


public class EnemyDodging : BaseBehaviour
{
    // 使用缓存的Animator参数
    public EnemyDodging(FSM machine) : base(machine) { }

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

public class EnemyStoping : EnemyGround
{
    private const float SpeedThreshold = 1f;
    private const int MaxAttackBack = 4;

    private int attackBack;

    public EnemyStoping(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        attackBack = Random.Range(0, MaxAttackBack);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (FSM.currentMoveSpeed < SpeedThreshold)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }

        if (FSM.AI.Attack && attackBack == 0)
        {
            FSM.TryChangeState(CharacterBehaviour.LandAttacking);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveSpeed(0, FSM.Status.MediumMoveDrag);
    }
}
#endregion

#region Air
public class EnemyAirborne : BaseBehaviour
{
    public EnemyAirborne(FSM machine) : base(machine) { }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveSpeed(0, FSM.Status.LightMoveDrag);
    }
}

public class EnemyFalling : PlayerAirborne
{
    public EnemyFalling(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade("Jump_Loop", 0.4f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Landing);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.Fall(bm.Status.GravityMultiplier);
    }
}

public class EnemyLanding : PlayerAirborne
{
    public EnemyLanding(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.verticalVelocity = 0;
        bm.Animator.CrossFade("Jump_End", 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (behaviourTimer > bm.Status.ActionCooldown)
        {
            bm.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveSpeed(0, bm.Status.MediumMoveDrag);

        bm.Float();
    }
}
#endregion

#region Combat
public class EnemyLandAttacking : BaseBehaviour
{
    // 缓存Animator参数
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");
    private static readonly int MoveHash = Animator.StringToHash("Move");

    bool attackRotate;

    private int attackCount;
    private Vector3 attackDirection;
    private bool hasAttacked;
    private bool canCombo;
    private int currentCombo = 0;
    private float attackDuration;
    private int canDodge;
    int combo;
    public EnemyLandAttacking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.verticalVelocity = 0;
        attackDirection = FSM.transform.forward;
        hasAttacked = false;
        canCombo = false;
        canDodge = Random.Range(0, 3);
        combo = Random.Range(0, 2);
        int a = Random.Range(0, FSM.AI.AttackType.Count);
        if (combo != 0)
        {
            attackCount = Random.Range(0, FSM.AI.AttackCount[a]) + 1;
        }
        else
        {
            attackCount = FSM.AI.AttackCount[a];
        }
        currentCombo = FSM.AI.AttackType[a];
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (FSM.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = FSM.Animator.GetFloat(CachedAnimator.OnAttack) != 0;
        }

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
            FSM.TryChangeState(canDodge == 0 && FSM.AI.Attack ? CharacterBehaviour.Dodging : CharacterBehaviour.Running);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;
        var animMove = bm.Animator.GetFloat(MoveHash);

        bm.UpdateMoveSpeed(animMove * bm.Status.HardMoveSpeed, bm.Status.HardMoveDrag);
        bm.UpdateRotate(bm.Status.HardRotateSpeed, attackDirection);
        bm.UpdateMoveDirection(bm.transform.forward);
        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        if (FSM.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = false;
        }

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

public class EnemySprintAttacking : BaseBehaviour
{
    private bool hasAttacked;
    private static readonly int SprintAttackHash = Animator.StringToHash("SprintAttack");
    private static readonly int OnAttackHash = Animator.StringToHash("OnAttack");

    public EnemySprintAttacking(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.verticalVelocity = 0;
        FSM.currentMoveSpeed = 0;
        hasAttacked = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (FSM.CombatSetting.WeaponCollider != null)
        {
            FSM.CombatSetting.WeaponCollider.enabled = FSM.Animator.GetFloat(CachedAnimator.OnAttack) != 0;
        }

        if (!hasAttacked)
        {
            FSM.Animator.CrossFade(SprintAttackHash, 0.2f, 1);
            FSM.Animator.CrossFade(SprintAttackHash, 0.2f, 2);
            hasAttacked = true;
        }
        else if (behaviourTimer > FSM.Status.SprintCombo.SprintAttackDuration)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        FSM.currentMoveSpeed = 0;
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}

public class EnemyDefencing : BaseBehaviour
{
    private bool hasDefenced;
    private bool hasBlocked;
    private bool canBlock;
    private static readonly int MoveHash = Animator.StringToHash("Move");
    private static readonly int BlockHash = Animator.StringToHash("Block_0");

    public EnemyDefencing(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        FSM.verticalVelocity = 0;
        hasDefenced = false;
        hasBlocked = false;
        canBlock = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        FSM.BufferDirection(false);

        if (!FSM.isGrounded)
        {
            FSM.TryChangeState(CharacterBehaviour.Falling);
        }

        if (!hasBlocked)
        {
            if (true)
            {
                behaviourTimer = 0;

                if (!hasDefenced)
                {
                    hasDefenced = true;
                    FSM.Animator.CrossFade(MoveHash, 0.2f, 1);
                    FSM.Animator.CrossFade(BlockHash, 0.2f, 2);
                }
            }
        }
        else if (canBlock && true)
        {
            hasBlocked = false;
            hasDefenced = false;
        }
        else if (behaviourTimer > FSM.Status.Block.BlockDuration)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }
}
#endregion

#region Hit
public class EnemySuffering : BaseBehaviour
{
    private const int HitDamage = 5;
    private int escape;

    public EnemySuffering(FSM machine) : base(machine) { }

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

        escape = Random.Range(0, 5);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (behaviourTimer > FSM.Status.Damage.HitDuration)
        {
            FSM.TryChangeState(escape == 0 && FSM.AI.Attack ? CharacterBehaviour.Dodging : CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

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
            <= 45 and >= -54 => "Hit_F",
            > 45 and <= 135 => "Hit_R",
            < -45 and >= -135 => "Hit_L",
            > 135 or < -135 => "Hit_B",
            _ => "Hit_F"
        };
    }
}
#endregion
