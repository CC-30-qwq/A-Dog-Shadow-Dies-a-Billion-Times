using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject Player;
    public FSM FSM;
    public new Camera camera;
    public GameObject playerSight;

    [field: SerializeField] private float mindistance;

    public float Yframe;
    public float Speed;
    public float povSmoothSpeed;

    [field: SerializeField] private bool hasLocked = false;
    [SerializeField] private Transform CurrentTarget;
    [SerializeField] private Transform centerTarget;
    [SerializeField] private Transform leftTarget;
    [SerializeField] private Transform rightTarget;

    public PlayerInputs Input { get; private set; }
    public CinemachineVirtualCamera Cinemachine { get; private set; }
    public CinemachinePOV POV { get; private set; }
    public PlayerVision Vision { get; private set; }

    private void Awake()
    {
        FSM = Player.GetComponent<FSM>();
        Cinemachine = GetComponent<CinemachineVirtualCamera>();
        POV = Cinemachine.GetCinemachineComponent<CinemachinePOV>();
        Input = GetComponent<PlayerInputs>();

        Vision = playerSight.GetComponent<PlayerVision>();
    }

    private void Update()
    {
        CameraLock();
    }

    private void FixedUpdate()
    {
        GetTarget();

        UpdateCamera();
    }

    private void CameraLock()
    {
        FSM.lockMove = hasLocked;

        if (CurrentTarget != null)
        {
            if (Input.LockIns)
            {
                centerTarget = CurrentTarget;

                if (!hasLocked)
                {
                    hasLocked = true;
                }
                else
                {
                    hasLocked = false;
                }
            }
        }

        if (Vision.Targets.Count == 0)
        {
            hasLocked = false;
        }

        if (hasLocked)
        {
            if (!Vision.Targets.Contains(centerTarget))
            {
                centerTarget = CurrentTarget;
                GetTarget();
            }
        }
    }
    private void UpdateCamera()
    {
        if (hasLocked)
        {
            if (centerTarget != null)
            {
                CalculateCameraDirection(centerTarget.Find("CharacterTarget"));
            }

            POV.m_HorizontalAxis.m_MaxSpeed = 0;
            POV.m_VerticalAxis.m_MaxSpeed = 0;

            POV.m_VerticalRecentering.m_enabled = false;
            POV.m_HorizontalRecentering.m_enabled = false;

            POV.m_VerticalAxis.m_InputAxisName = null;
            POV.m_HorizontalAxis.m_InputAxisName = null;
        }
        else
        {
            centerTarget = null;

            POV.m_HorizontalAxis.m_MaxSpeed = Speed;
            POV.m_VerticalAxis.m_MaxSpeed = Speed;

            POV.m_VerticalRecentering.m_enabled = RecenterMethod();
            POV.m_HorizontalRecentering.m_enabled = RecenterMethod();

            POV.m_VerticalAxis.m_InputAxisName = "Mouse Y";
            POV.m_HorizontalAxis.m_InputAxisName = "Mouse X";
        }
    }

    private bool RecenterMethod()
    {
        return FSM.currentMoveSpeed >= FSM.Status.MediumMoveSpeed - 1 && POV.m_VerticalAxis.m_InputAxisValue == 0 && POV.m_HorizontalAxis.m_InputAxisValue == 0;
    }

    #region Methods
    private void GetTarget()
    {
        if (Vision.Targets.Count != 0)
        {
            if (Vision.Targets.Contains(CurrentTarget))
            {
                mindistance = CalculateEnemyDistance(CurrentTarget.Find("CharacterTarget"));
            }
            else
            {
                mindistance = playerSight.GetComponent<SphereCollider>().radius;
                CurrentTarget = Vision.Targets[0];
            }

            foreach (Transform enemy in Vision.Targets)
            {
                float distance = CalculateEnemyDistance(enemy.Find("CharacterTarget"));

                if (distance < mindistance)
                {
                    CurrentTarget = enemy;
                }
            }
        }
        else
        {
            CurrentTarget = null;
        }

        if (centerTarget != null)
        {
            Vector3 Direction = centerTarget.transform.Find("CharacterTarget").position - Player.transform.Find("CharacterTarget").position;
            Direction.y = 0;
            FSM.targetDirection = Direction.normalized;
        }
    }
    private float CalculateEnemyDistance(Transform target)
    {
        Vector3 lineDirection = camera.transform.forward.normalized;

        Vector3 pointToStart = target.position - playerSight.transform.position;

        // �������ֱ�߷��������ϵ�ͶӰ����
        float projectionLength = Vector3.Dot(pointToStart, lineDirection);
        if (projectionLength < 0)
        {
            projectionLength = playerSight.GetComponent<SphereCollider>().radius;
        }
        else
        {
            projectionLength = Vector3.Dot(pointToStart, lineDirection);
        }
        // �ҵ�ֱ���Ͼ��������ĵ�
        Vector3 closestPointOnLine = playerSight.transform.position + projectionLength * camera.transform.forward;
        // ���ص㵽�����ľ���
        float distance = Vector3.Distance(target.position, closestPointOnLine);

        return distance;
    }
    private void CalculateCameraDirection(Transform target)
    {
        // ���������Ƕȣ�ˮƽ����ֱ�������� LerpAngle ƽ���� POV �ᣬ��������
        Vector3 camPos = transform.position;
        Vector3 dir = target.position - camPos;
        dir.y = 0f;

        // ˮƽ��ע��ʹ�� Atan2(dx, dz) ��֮ǰ����һ�£���ʹ�� LerpAngle ƽ��
        float desiredHoriz = Mathf.Atan2(target.position.x - camPos.x, target.position.z - camPos.z) * Mathf.Rad2Deg;
        float smoothHoriz = Mathf.LerpAngle(POV.m_HorizontalAxis.Value, desiredHoriz, Time.deltaTime * povSmoothSpeed);
        POV.m_HorizontalAxis.Value = smoothHoriz;

        // ��ֱ������ԭ�� Yframe �߼�����ƽ����ֵ
        float targetAngleY = Mathf.Atan2(target.position.y - camPos.y, (target.position - camPos).magnitude) * Mathf.Rad2Deg;
        float desiredVert = Yframe - targetAngleY;
        float smoothVert = Mathf.LerpAngle(POV.m_VerticalAxis.Value, desiredVert, Time.deltaTime * povSmoothSpeed);
        POV.m_VerticalAxis.Value = smoothVert;
    }
    #endregion
}
