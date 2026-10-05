using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    #region Inputs
    [field: SerializeField] private bool LockInput;
    [field: SerializeField] private Vector2 MoveInput;
    [field: SerializeField] private bool DodgeInput;
    [field: SerializeField] private bool SprintInput;
    [field: SerializeField] private bool JumpInput;
    [field: SerializeField] private bool AttackInput;
    [field: SerializeField] private bool ChargeInput;
    [field: SerializeField] private bool DefenceInput;
    [field: SerializeField] private bool Skill_0Input;
    [field: SerializeField] private bool Skill_1Input;
    #endregion

    #region Timers
    private float DodgeTimer;
    private float SprintTimer;
    private float JumpTimer;
    private float AttackTimer;
    #endregion

    #region Ins Outputs
    public bool LockIns;
    public Vector2 MoveIns;
    public bool DodgeIns;
    public bool SprintIns;
    public bool JumpIns;
    public bool AttackIns;
    public bool ChargeIns;
    public bool DefenceIns;
    public bool Skill_0Ins;
    public bool Skill_1Ins;
    #endregion

    private void Update()
    {
        GetInput();

        LockIns = LockInput;
        BufferMoveInput(0.5f);
        BufferDodgeInput(0.3f);
        DelaySprintInput(0.2f);
        BufferJumpInput(0.3f);
        BufferAttackInput(0.3f);
        ChargeIns = ChargeInput;
        DefenceIns = DefenceInput;
        Skill_0Ins = Skill_0Input;
        Skill_1Ins = Skill_1Input;
    }

    #region Methods
    private void GetInput()
    {
        LockInput = Input.GetButtonDown("Lock");
        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        DodgeInput = Input.GetButtonDown("Sprint");
        SprintInput = Input.GetButton("Sprint");
        JumpInput = Input.GetButtonDown("Jump");
        AttackInput = Input.GetButtonDown("Attack");
        ChargeInput = Input.GetButton("Attack");
        DefenceInput = Input.GetButton("Block");
        Skill_0Input = Input.GetButtonDown("Skill_0");
        Skill_1Input = Input.GetButtonDown("Skill_1");
    }

    #region Buffer Methods
    private void BufferMoveInput(float moveDamp)
    {
        MoveIns = MoveIns.sqrMagnitude == 0f && MoveInput.sqrMagnitude > 0f ? MoveInput : Vector2.Lerp(MoveIns, MoveInput, moveDamp);
        if (MoveInput.sqrMagnitude == 0f) MoveIns = Vector2.Scale(MoveIns, new Vector2(Mathf.Abs(MoveIns.x) >= 0.1f ? 1f : 0f, Mathf.Abs(MoveIns.y) >= 0.1f ? 1f : 0f));
    }

    private void DelaySprintInput(float delayTime)
    {
        if (SprintInput)
        {
            SprintTimer += Time.deltaTime;
            // Only enable sprint output after holding for delayTime
            SprintIns = SprintTimer > delayTime;
        }
        else
        {
            SprintTimer = 0f;
            SprintIns = false;
        }
    }

    private void BufferDodgeInput(float bufferTime)
    {
        if (DodgeInput)
        {
            DodgeTimer = 0f;
            DodgeIns = true;
            return;
        }

        DodgeTimer += Time.deltaTime;
        if (DodgeTimer > bufferTime)
        {
            DodgeIns = false;
        }
    }

    private void BufferJumpInput(float bufferTime)
    {
        if (JumpInput)
        {
            JumpTimer = 0f;
            JumpIns = true;
            return;
        }

        JumpTimer += Time.deltaTime;
        if (JumpTimer > bufferTime)
        {
            JumpIns = false;
        }
    }

    private void BufferAttackInput(float bufferTime)
    {
        if (AttackInput)
        {
            AttackTimer = 0f;
            AttackIns = true;
            return;
        }

        AttackTimer += Time.deltaTime;
        if (AttackTimer > bufferTime)
        {
            AttackIns = false;
        }
    }
    #endregion
    #endregion
}
