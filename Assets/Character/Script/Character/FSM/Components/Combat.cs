using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Combat
{
    public GameObject Weapon;
    public float WeaponMass;
    public CapsuleCollider WeaponCollider;
    public Vector3 attackDection;
    public LayerMask targetMask;
}
