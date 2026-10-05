using System.Collections.Generic;
using UnityEngine;

public class BehaviourStatus : MonoBehaviour
{
    [Header("Movement")]
    public float LightMoveSpeed;
    public float MediumMoveSpeed;
    public float HardMoveSpeed;

    public float LightMoveDrag;
    public float MediumMoveDrag;
    public float HardMoveDrag;

    public float LightRotateSpeed;
    public float MediumRotateSpeed;
    public float HardRotateSpeed;

    public float JumpForce;
    public float JumpHeight;
    public float GravityMultiplier;

    [Header("Combat")]
    public float AttackCancelTime;
    public float ChargeTime;
    public float AttackGetRotateTime;
    public List<NormalCombos> NormalCombo;
    public SprintCombo SprintCombo;
    public List<AirCombos> AirCombo;
    public Block Block;
    public Dodge Dodge;
    public Damage Damage;
    public Skill Skill;

    public float ActionCooldown;
}