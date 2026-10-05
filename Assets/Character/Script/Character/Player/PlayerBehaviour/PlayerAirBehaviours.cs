using UnityEngine;

public class PlayerAirborne : BaseBehaviour
{
    public PlayerAirborne(FSM machine) : base(machine) { }

    public override void UpdateState()
    {
        base.UpdateState();

        FSM.BufferDirection(false);
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        FSM.UpdateMoveSpeed(0, FSM.Status.LightMoveDrag);
    }

    public override void ExitState()
    {
        base.ExitState();

        FSM.JumpDistance = 0;
    }
}

public class PlayerJumping : PlayerAirborne
{
    Vector3 jumpDirection;
    bool hasJumped;

    public PlayerJumping(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        hasJumped = false;
        FSM.Animator.CrossFade(CachedAnimator.Jump_Start, 0.2f, 1);

        if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
        {
            bm.JumpDistance = bm.currentMoveSpeed + bm.Status.JumpForce;
        }
        else
        {
            bm.JumpDistance = bm.currentMoveSpeed;
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (behaviourTimer > bm.Status.ActionCooldown)
        {
            if (!hasJumped)
            {
                bm.verticalVelocity = Mathf.Sqrt(bm.Status.JumpHeight * bm.Status.GravityMultiplier * -2f * Physics.gravity.y);
                hasJumped = true;
            }

            if (bm.verticalVelocity < 0)
            {
                bm.TryChangeState(CharacterBehaviour.Falling);
            }

            if (bm.Input.AttackIns && !bm.isGrounded)
            {
                bm.TryChangeState(CharacterBehaviour.AirAttacking);
            }
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        if (behaviourTimer > bm.Status.ActionCooldown)
        {
            bm.moveDirection = jumpDirection;

            bm.UpdateMoveSpeed(bm.JumpDistance, bm.Status.MediumMoveDrag);

            bm.Fall(bm.Status.GravityMultiplier);

            if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
            {
                bm.TransformPosition(bm.Status.LightMoveDrag);

                if (!bm.lockMove)
                {
                    bm.UpdateRotate(bm.Status.LightRotateSpeed, bm.InputDirection);
                }
                else
                {
                    bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
                }
            }
        }
        else
        {
            if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
            {
                jumpDirection = bm.InputDirection;
            }

            bm.UpdateMoveSpeed(0, bm.Status.MediumMoveDrag);
        }
    }
}

public class PlayerFalling : PlayerAirborne
{
    public PlayerFalling(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Jump_Loop, 0.4f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Landing);
        }

        if (bm.Input.AttackIns)
        {
            bm.TryChangeState(CharacterBehaviour.AirAttacking);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.Fall(bm.Status.GravityMultiplier);

        if (bm.Input.MoveIns.sqrMagnitude > 0.9f)
        {
            bm.TransformPosition(bm.Status.LightMoveDrag);

            if (!bm.lockMove)
            {
                bm.UpdateRotate(bm.Status.LightRotateSpeed, bm.InputDirection);
            }
            else
            {
                bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
            }
        }
    }
}

public class PlayerLanding : PlayerAirborne
{
    bool HardLand;

    public PlayerLanding(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Jump_End, 0.2f, 1);

        HardLand = bm.verticalVelocity > Physics.gravity.y;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        bm.verticalVelocity = 0;

        if (HardLand)
        {
            if (behaviourTimer > bm.Status.ActionCooldown)
            {
                bm.TryChangeState(CharacterBehaviour.Idling);
            }
        }
        else
        {
            if (behaviourTimer > bm.Status.ActionCooldown * 2)
            {
                bm.TryChangeState(CharacterBehaviour.Idling);
            }
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveSpeed(0, bm.Status.MediumMoveDrag);

        if (!bm.lockMove)
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.BufferedDirection);
        }
        else
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
        }

        bm.Float();
    }
}