using System.Collections;
using UnityEngine;

public class PlayerGround : BaseBehaviour
{
    public PlayerGround(FSM machine) : base(machine) { }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        bm.BufferDirection(!bm.lockMove);

        if (!bm.isGrounded)
        {
            bm.verticalVelocity = -5;
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        if (bm.Input.DefenceIns)
        {
            bm.TryChangeState(CharacterBehaviour.Blocking);
        }
        else if (bm.Input.AttackIns)
        {
            if (bm.currentMoveSpeed > bm.Status.MediumMoveSpeed)
            {
                bm.TryChangeState(CharacterBehaviour.SprintAttacking);
            }
            else
            {
                bm.TryChangeState(CharacterBehaviour.LandAttacking);
            }
        }


        if (bm.Input.JumpIns)
        {
            bm.TryChangeState(CharacterBehaviour.Jumping);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.Float();
    }
}

public class PlayerIdle : PlayerGround
{
    bool hasIdled;
    public PlayerIdle(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        hasIdled = false;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!hasIdled && behaviourTimer > bm.Status.ActionCooldown)
        {
            hasIdled = true;
            bm.Animator.CrossFade(CachedAnimator.Idle, 0.1f, 1);
        }

        if (bm.Input.MoveIns != Vector2.zero)
        {
            bm.TryChangeState(CharacterBehaviour.Starting);
        }

        if (bm.Input.DodgeIns)
        {
            bm.TryChangeState(CharacterBehaviour.Dodging);
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
    }
}

public class PlayerStarting : PlayerGround
{
    public PlayerStarting(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Start, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (bm.Input.MoveIns != Vector2.zero)
        {
            if (!bm.lockMove)
            {
                if (bm.Input.SprintIns || bm.Input.DodgeIns)
                {
                    bm.TryChangeState(CharacterBehaviour.Dodging);
                }
                else if (behaviourTimer > bm.Status.ActionCooldown / 2)
                {
                    bm.TryChangeState(CharacterBehaviour.Running);
                }
            }
            else
            {
                if (bm.Input.DodgeIns)
                {
                    bm.TryChangeState(CharacterBehaviour.Dodging);
                }
                else if (behaviourTimer > bm.Status.ActionCooldown / 2)
                {
                    bm.TryChangeState(CharacterBehaviour.Running);
                }
            }
        }

        if (bm.Input.MoveIns.sqrMagnitude < 0.5f && behaviourTimer > bm.Status.ActionCooldown / 2)
        {
            bm.TryChangeState(CharacterBehaviour.Stoping);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        if (bm.Input.MoveIns != Vector2.zero)
        {
            bm.UpdateMoveSpeed(bm.Status.LightMoveSpeed, bm.Status.LightMoveDrag);
        }
        else
        {
            bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
        }

        if (bm.InputDirection != Vector3.zero)
        {
            bm.UpdateMoveDirection(bm.InputDirection);
        }

        if (!bm.lockMove)
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.BufferedDirection);
        }
        else
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
        }
    }
}

public class PlayerRunning : PlayerGround
{
    public PlayerRunning(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Run, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (bm.Input.DodgeIns)
        {
            bm.TryChangeState(CharacterBehaviour.Dodging);
        }

        if (bm.Input.MoveIns.sqrMagnitude < 0.5f)
        {
            bm.TryChangeState(CharacterBehaviour.Stoping);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveSpeed(bm.Status.MediumMoveSpeed, bm.Status.MediumMoveDrag);

        bm.UpdateMoveDirection(bm.InputDirection);

        if (!bm.lockMove)
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.BufferedDirection);
        }
        else
        {
            bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
        }
    }
}

public class PlayerDodging : BaseBehaviour
{
    Vector3 dodgeDirection;
    bool hasDodged;

    public PlayerDodging(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        hasDodged = false;

        bm.Animator.CrossFade(CachedAnimator.Avoid, 0.2f, 1);

        if (bm.InputDirection == Vector3.zero)
        {
            dodgeDirection = bm.transform.forward;
        }
        else
        {
            dodgeDirection = bm.InputDirection;
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (!bm.isGrounded)
        {
            bm.TryChangeState(CharacterBehaviour.Falling);
        }

        if (!hasDodged)
        {
            if (behaviourTimer > bm.Status.Dodge.DodgeDuration)
            {
                StartStateCoroutine(EnableDodgedWindow());
            }
        }
        else if (behaviourTimer > FSM.Status.Dodge.DodgeDuration + FSM.Status.ActionCooldown)
        {
            FSM.TryChangeState(CharacterBehaviour.Idling);
        }
        else if (behaviourTimer > FSM.Status.Dodge.DodgeDuration)
        {
            bm.BufferDirection(true);

            FSM.Animator.CrossFade(CachedAnimator.HardStop, 0.2f, 1, 0.5f);

            if (bm.Input.DefenceIns)
            {
                bm.TryChangeState(CharacterBehaviour.Blocking);
            }
            else if (bm.Input.AttackIns)
            {
                if (bm.currentMoveSpeed > bm.Status.MediumMoveSpeed)
                {
                    bm.TryChangeState(CharacterBehaviour.SprintAttacking);
                }
                else
                {
                    bm.TryChangeState(CharacterBehaviour.LandAttacking);
                }
            }


            if (bm.Input.JumpIns)
            {
                bm.TryChangeState(CharacterBehaviour.Jumping);
            }
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        if (behaviourTimer < bm.Status.Dodge.DodgeDuration / 2)
        {
            bm.UpdateMoveSpeed(bm.Status.Dodge.DodgeSpeed, bm.Status.MediumMoveDrag);
        }
        else if (behaviourTimer < bm.Status.Dodge.DodgeDuration + bm.Status.ActionCooldown)
        {
            bm.UpdateMoveSpeed(0, bm.Status.MediumMoveDrag);
        }

        bm.UpdateMoveDirection(dodgeDirection);

        if (!bm.lockMove)
        {
            bm.UpdateRotate(bm.Status.MediumRotateSpeed, dodgeDirection);
        }
        else
        {
            bm.UpdateRotate(bm.Status.MediumRotateSpeed, bm.targetDirection);
        }

        bm.Float();
    }

    public override void ExitState()
    {
        base.ExitState();

        var bm = FSM;

        bm.Animator.SetFloat("Move", 0);
    }

    private IEnumerator EnableDodgedWindow()
    {
        if (FSM.Input.SprintIns && FSM.Input.MoveIns != Vector2.zero)
        {
            FSM.TryChangeState(CharacterBehaviour.Sprinting);
        }
        yield return null;
        hasDodged = true;
    }
}

public class PlayerSprinting : PlayerGround
{
    public PlayerSprinting(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        bm.Animator.CrossFade(CachedAnimator.Run, 0.2f, 1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if (bm.Input.MoveIns.sqrMagnitude < 0.5f)
        {
            bm.TryChangeState(CharacterBehaviour.Stoping);
        }

        if (!bm.Input.SprintIns && bm.currentMoveSpeed <= bm.Status.MediumMoveSpeed)
        {
            bm.TryChangeState(CharacterBehaviour.Running);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        bm.UpdateMoveDirection(bm.transform.forward);

        if (!bm.Input.SprintIns)
        {
            bm.UpdateMoveSpeed(bm.Status.LightMoveSpeed, bm.Status.MediumMoveDrag);
        }
        else
        {
            bm.UpdateMoveSpeed(bm.Status.HardMoveSpeed, bm.Status.MediumMoveDrag);
        }

        if (!bm.lockMove)
        {
            bm.UpdateRotate(bm.Status.MediumRotateSpeed, bm.BufferedDirection);
        }
        else
        {
            bm.UpdateRotate(bm.Status.MediumRotateSpeed, bm.InputDirection);
        }
    }
}

public class PlayerStoping : PlayerGround
{
    bool canUpdateRotate;
    public PlayerStoping(FSM machine) : base(machine) { }

    public override void EnterState()
    {
        base.EnterState();

        var bm = FSM;

        canUpdateRotate = bm.currentMoveSpeed < bm.Status.MediumMoveSpeed;

        if (canUpdateRotate)
        {
            bm.Animator.CrossFade(CachedAnimator.LightStop, 0.2f, 1);
        }
        else
        {
            bm.Animator.CrossFade(CachedAnimator.HardStop, 0.2f, 1);
        }
    }

    public override void UpdateState()
    {
        base.UpdateState();

        var bm = FSM;

        if ((canUpdateRotate && behaviourTimer > bm.Status.ActionCooldown) || (!canUpdateRotate && behaviourTimer > bm.Status.ActionCooldown * 2))
        {
            bm.TryChangeState(CharacterBehaviour.Idling);
        }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        var bm = FSM;

        if (canUpdateRotate)
        {
            if (bm.InputDirection != Vector3.zero && behaviourTimer > bm.Status.ActionCooldown / 2)
            {
                bm.UpdateMoveDirection(bm.InputDirection);
            }

            if (!bm.lockMove)
            {
                bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.BufferedDirection);
            }
            else
            {
                bm.UpdateRotate(bm.Status.HardRotateSpeed, bm.targetDirection);
            }

            if (bm.Input.MoveIns != Vector2.zero)
            {
                bm.UpdateMoveSpeed(bm.Status.LightMoveSpeed, bm.Status.LightMoveDrag);
            }
            else
            {
                bm.UpdateMoveSpeed(0, bm.Status.HardMoveDrag);
            }
        }
        else
        {
            bm.UpdateMoveSpeed(0, bm.Status.MediumMoveDrag);
        }
    }
}
