public class PlayerSuffering : BaseBehaviour
{
    public PlayerSuffering(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;
        var animKey = GetHitAnimKey(bm.hitAngle);

        bm.verticalVelocity = 0;

        bm.Animator.CrossFade(CombosHashCache.Get(animKey), 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (behaviourTimer > bm.Status.Damage.SufferDuration)
        {
            if (bm.Input.DefenceIns)
            {
                bm.TryChangeState(CharacterBehaviour.Defencing);
            }
            else
            {
                bm.TryChangeState(CharacterBehaviour.Idling);
            }
        }

        if (behaviourTimer > bm.Status.Damage.SufferCoolDown)
        {
            bm.hasDamaged = false;
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

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Idle, 0.2f, 1);

        bm.currentMoveSpeed = 0;
    }

    private static string GetHitAnimKey(float angle)
    {
        if (angle <= 30 && angle >= -30)
        {
            return "Hit_F";
        }
        if (angle > 30 && angle <= 90)
        {
            return "Hit_FR";
        }
        if (angle > 90 && angle <= 150)
        {
            return "Hit_R";
        }
        if (angle < -30 && angle >= -90)
        {
            return "Hit_FL";
        }
        if (angle < -90 && angle >= -150)
        {
            return "Hit_L";
        }
        if (angle > 150 || angle < -150)
        {
            return "Hit_B";
        }
        return "Hit_F";
    }
}

public class PlayerParring : BaseBehaviour
{
    public PlayerParring(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;
        var animKey = GetHitAnimKey(bm.hitAngle);

        bm.verticalVelocity = 0;

        bm.Animator.CrossFade(CombosHashCache.Get(animKey), 0.1f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (behaviourTimer > bm.Status.Damage.ParryDuration)
        {
            if (bm.Input.DefenceIns)
            {
                bm.TryChangeState(CharacterBehaviour.Defencing);
            }
            else
            {
                bm.TryChangeState(CharacterBehaviour.Idling);
            }
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveSpeed(bm.Status.Damage.moveSpeeds[1].moveSpeed, bm.Status.HardMoveDrag);

        bm.UpdateMoveDirection(-bm.DamageDirection);

        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.currentMoveSpeed = 0;
    }

    private static string GetHitAnimKey(float angle)
    {
        if (angle > 0 && angle <= 30)
        {
            return "Parry_1";
        }
        if (angle > 30 && angle <= 90)
        {
            return "Parry_0";
        }
        if (angle < 0 && angle >= -30)
        {
            return "Parry_2";
        }
        if (angle < -30 && angle >= -90)
        {
            return "Parry_3";
        }
        else
        {
            return "Parry_0";
        }
    }
}

public class PlayerHiting : BaseBehaviour
{
    public PlayerHiting(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;
        var animKey = GetHitAnimKey(bm.hitAngle);

        bm.verticalVelocity = 0;

        bm.Animator.CrossFade(CombosHashCache.Get(animKey), 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (behaviourTimer > bm.Status.Damage.HitDuration)
        {
            if (bm.Input.DefenceIns)
            {
                bm.TryChangeState(CharacterBehaviour.Defencing);
            }
            else
            {
                bm.Animator.CrossFade(CachedAnimator.Block_1, 0.2f, 1);

                bm.TryChangeState(CharacterBehaviour.Idling);
            }
        }

        if (behaviourTimer > bm.Status.Damage.SufferCoolDown)
        {
            bm.hasDamaged = false;
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveSpeed(bm.Status.Damage.moveSpeeds[2].moveSpeed, bm.Status.HardMoveDrag);

        bm.UpdateMoveDirection(-bm.DamageDirection);

        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.currentMoveSpeed = 0;
    }

    private static string GetHitAnimKey(float angle)
    {
        if (angle > 0 && angle <= 90)
        {
            return "DefenseHit_0";
        }
        if (angle < 0 && angle >= -90)
        {
            return "DefenseHit_1";
        }
        else
        {
            return "DefenseHit_0";
        }
    }
}
