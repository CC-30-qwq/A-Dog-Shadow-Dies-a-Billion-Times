using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(FSM))]
public class FootIK : MonoBehaviour
{
    [Header("IK Settings")]
    public bool canIK;
    public float weightLerp;
    public float ikWeightL;
    public float ikWeightR;
    [Range(0, 1)]
    public float heightFromGround = 0.1f; // �������ĸ߶�
    public float rotateOffset;

    [Header("Raycast Settings")]
    public LayerMask groundLayerMask = 1; // Ͷ�������Ĳ�
    public float raycastStartHeight = 0.5f; // �ӽ����Ϸ���ߴ���ʼͶ��
    public float raycastDistance = 1f; // ���߳���

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateIKweight();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (canIK)
        {
            HandleFootIK(AvatarIKGoal.LeftFoot, animator.GetBoneTransform(HumanBodyBones.LeftFoot), ikWeightL);
            HandleFootIK(AvatarIKGoal.RightFoot, animator.GetBoneTransform(HumanBodyBones.RightFoot), ikWeightR);
        }
    }

    void HandleFootIK(AvatarIKGoal foot, Transform footBone, float ikWeight)
    {
        Vector3 footPosition = animator.GetIKPosition(foot);

        Vector3 raycastOrigin = footPosition + Vector3.up * raycastStartHeight;

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            Vector3 targetPosition = hit.point + Vector3.up * heightFromGround;
            Vector3 footForward = Quaternion.Euler(0, rotateOffset, 0) * footBone.forward;
            Vector3 projectedForward = Vector3.ProjectOnPlane(footForward, hit.normal).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, hit.normal);

            animator.SetIKPositionWeight(foot, ikWeight);
            animator.SetIKRotationWeight(foot, ikWeight);

            animator.SetIKPosition(foot, targetPosition);
            animator.SetIKRotation(foot, targetRotation);
        }
        else
        {
            animator.SetIKPositionWeight(foot, 0);
            animator.SetIKRotationWeight(foot, 0);
        }
    }
    private void UpdateIKweight()
    {
        if (CanUpdateIKweight(AvatarIKGoal.LeftFoot))
        {
            ikWeightL = Mathf.Lerp(ikWeightL, 1, Time.deltaTime * weightLerp);
        }
        else
        {
            ikWeightL = Mathf.Lerp(ikWeightL, 0, Time.deltaTime * weightLerp);
        }

        if (CanUpdateIKweight(AvatarIKGoal.RightFoot))
        {
            ikWeightR = Mathf.Lerp(ikWeightR, 1, Time.deltaTime * weightLerp);
        }
        else
        {
            ikWeightR = Mathf.Lerp(ikWeightR, 0, Time.deltaTime * weightLerp);
        }
    }

    bool CanUpdateIKweight(AvatarIKGoal foot)
    {
        Vector3 footPosition = animator.GetIKPosition(foot);

        Vector3 raycastOrigin = footPosition + Vector3.up * raycastStartHeight;

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, raycastDistance, groundLayerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void DrawFootGizmo(AvatarIKGoal foot)
    {
        Vector3 footPosition = animator.GetIKPosition(foot);
        Vector3 raycastOrigin = footPosition + Vector3.up * raycastStartHeight;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + Vector3.down * raycastDistance);
    }
    void OnDrawGizmosSelected()
    {
        if (animator == null) return;

        DrawFootGizmo(AvatarIKGoal.LeftFoot);
        DrawFootGizmo(AvatarIKGoal.RightFoot);
    }
}
