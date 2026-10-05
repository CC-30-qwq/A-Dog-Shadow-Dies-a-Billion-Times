using UnityEngine;

// Character State Enum
public enum CharacterState
{
    Normal,
    Parring,
    Hiting,
    Invincible,
    Uninterrupt,
    Dead
}

[RequireComponent(typeof(FSM))]

// State Machine
public class StateMachine : MonoBehaviour
{
    public FSM FSM;
    public GameObject Model;
    public CharacterState currentState;
    public bool canEnterState = false;
    public float Timer;

    private void Start()
    {
        currentState = CharacterState.Normal;
        FSM = GetComponent<FSM>();
    }

    private void Update()
    {
        EnterState(CharacterBehaviour.Dodging, CharacterState.Invincible, CharacterState.Normal, 0.2f);
        EnterState(CharacterBehaviour.Defencing, CharacterState.Parring, CharacterState.Hiting, 0.2f);
        EnterState(CharacterBehaviour.UsingSkill_0, CharacterState.Hiting, CharacterState.Normal, 0);

        if (FSM.currentBehaviourType == CharacterBehaviour.Idling)
        {
            currentState = CharacterState.Normal;
        }

        if (currentState == CharacterState.Normal)
        {
            canEnterState = true;
            Timer = 0;
        }

        if (FSM.Health.HP <= 0)
        {
            currentState = CharacterState.Dead;
            StateUpdate();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Vector3 closestPointOnThis = GetComponent<Collider>().ClosestPoint(other.transform.position);

            FSM.DamageDirection = (closestPointOnThis - transform.GetComponent<Collider>().bounds.center).normalized;
            FSM.DamageDirection.y = 0;
            FSM.hitAngle = Vector3.SignedAngle(transform.forward, FSM.DamageDirection, Vector3.up);
        }
    }

    #region UpdateState
    public void StateUpdate()
    {
        if (currentState == CharacterState.Dead)
        {
            FSM.GetComponent<Collider>().enabled = false;
            Model.transform.parent = null;
            FSM.Animator.enabled = false;
            FSM.enabled = false;
            if (FSM.CombatSetting.Weapon != null)
            {
                FSM.CombatSetting.Weapon.GetComponent<Collider>().isTrigger = false;
                if (FSM.CombatSetting.Weapon.GetComponent<Rigidbody>() != null)
                {
                    FSM.CombatSetting.Weapon.GetComponent<Rigidbody>().mass = FSM.CombatSetting.WeaponMass;
                    FSM.CombatSetting.Weapon.GetComponent<Rigidbody>().drag = FSM.CombatSetting.WeaponMass;
                }
                else
                {
                    FSM.CombatSetting.Weapon.AddComponent<Rigidbody>();
                }
            }
            if (FSM.CombatSetting.WeaponCollider != null)
            {
                FSM.CombatSetting.WeaponCollider.enabled = false;
            }
        }

        if (!FSM.hasDamaged)
        {
            if (currentState == CharacterState.Normal)
            {
                FSM.hasDamaged = true;

                if (FSM.Health != null)
                {
                    FSM.Health.HP -= 20;
                }

                FSM.TryChangeState(CharacterBehaviour.Suffering);
            }
            else if (currentState == CharacterState.Parring)
            {
                if (Mathf.Abs(FSM.hitAngle) < 90)
                {
                    FSM.TryChangeState(CharacterBehaviour.Parring);
                }
                else
                {
                    FSM.hasDamaged = true;

                    if (FSM.Health != null)
                    {
                        FSM.Health.HP -= 20;
                    }

                    FSM.TryChangeState(CharacterBehaviour.Suffering);
                }
            }
            else if (currentState == CharacterState.Hiting)
            {
                if (Mathf.Abs(FSM.hitAngle) < 90)
                {
                    FSM.hasDamaged = true;

                    if (FSM.Health != null)
                    {
                        FSM.Health.HP -= 2;
                    }

                    FSM.TryChangeState(CharacterBehaviour.Hiting);
                }
                else
                {
                    FSM.hasDamaged = true;

                    if (FSM.Health != null)
                    {
                        FSM.Health.HP -= 20;
                    }

                    FSM.TryChangeState(CharacterBehaviour.Suffering);
                }
            }
        }
    }
    #endregion

    #region ChangeState
    private void EnterState(CharacterBehaviour behaviour, CharacterState enterState, CharacterState exitState, float stateTime)
    {
        if (stateTime == 0)
        {
            if (FSM.currentBehaviourType == behaviour)
            {
                currentState = exitState;
            }
        }
        else
        {
            if (FSM.currentBehaviourType == behaviour)
            {
                if (canEnterState)
                {
                    canEnterState = false;
                    currentState = enterState;
                }
            }

            if (currentState == enterState)
            {
                Timer += Time.deltaTime;
                if (Timer > stateTime)
                {
                    currentState = exitState;
                }
            }
        }
    }
    #endregion
}