using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIIns
{
    Standoff,
    Move,
    Attack,
    Dodge,
    Defence
}
public class CharacterAI : MonoBehaviour
{
    public GameObject EnemySight;
    public FSM behaviourMachine;

    [field: SerializeField] protected AIIns CurrentIns { get; private set; }
    [field: SerializeField] public Transform CurrentTarget { get; private set; }
    [field: SerializeField] public float TargetDistance { get; private set; }

    public float LightDistance;
    public float MediumDistance;
    public float HardDistance;

    public List<int> AttackType;
    public List<int> AttackCount;

    #region ���
    [field: SerializeField] public bool Stop { get; private set; }
    [field: SerializeField] public bool Standoff { get; private set; }
    [field: SerializeField] public bool Move { get; private set; }
    [field: SerializeField] public bool Attack { get; private set; }
    [field: SerializeField] public bool Dodge { get; private set; }
    [field: SerializeField] public bool Defence { get; private set; }
    #endregion

    public NavMeshAgent Agent { get; private set; }
    public EnemyVision Vision { get; private set; }

    private void Awake()
    {
        behaviourMachine = GetComponent<FSM>();
        Agent = GetComponent<NavMeshAgent>();

        Vision = EnemySight.GetComponent<EnemyVision>();
    }
    private void OnValidate()
    {
        if (LightDistance > MediumDistance)
        {
            LightDistance = MediumDistance;
        }
        else if (MediumDistance > HardDistance)
        {
            MediumDistance = HardDistance;
        }
    }

    private void Update()
    {
        Agent.updatePosition = false;
        Agent.updateRotation = false;

        Attack = OutPutIns(AIIns.Attack, Attack);
        Standoff = OutPutIns(AIIns.Standoff, Standoff);
        Move = OutPutIns(AIIns.Move, Move);

        DistanceIns();
    }

    private void FixedUpdate()
    {
        UpdateTarget();
    }

    #region 内部方法
    private void DistanceIns()
    {
        if (TargetDistance <= LightDistance)
        {
            CurrentIns = AIIns.Attack;
        }
        else if (TargetDistance <= MediumDistance)
        {
            CurrentIns = AIIns.Standoff;
        }
        else if (TargetDistance <= HardDistance)
        {
            CurrentIns = AIIns.Move;
        }
    }
    private bool OutPutIns(AIIns aiIns, bool ins)
    {
        if (CurrentIns == aiIns)
        {
            ins = true;
        }
        else
        {
            ins = false;
        }
        return ins;
    }
    private void UpdateTarget()
    {
        if (Vision.Targets.Count != 0)
        {
            CurrentTarget = Vision.Targets[0];

            behaviourMachine.targetDirection = (CurrentTarget.Find("CharacterTarget").position - transform.Find("CharacterTarget").position).normalized;
            TargetDistance = (CurrentTarget.Find("CharacterTarget").position - transform.Find("CharacterTarget").position).magnitude;
        }
    }
    #endregion
}
