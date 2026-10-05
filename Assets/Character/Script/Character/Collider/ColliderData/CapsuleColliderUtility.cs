using System;
using UnityEngine;

[Serializable]
public class CapsuleColliderUtility
{
    // CapsuleColliderData
    public CapsuleCollider CapsuleCollider { get; private set; }
    public Vector3 ColliderCenterInLocalSpace { get; private set; }

    // DefaultColliderData
    [field: SerializeField] public float Height { get; private set; }
    [field: SerializeField] public float CenterY { get; private set; }
    [field: SerializeField] public float Radius { get; private set; }

    // SlopeData
    [field: SerializeField][field: Range(0f, 1f)] public float StepHeightPercentage { get; private set; }
    [field: SerializeField][field: Range(0f, 5f)] public float FloatRayDistance { get; private set; }
    [field: SerializeField][field: Range(0f, 50f)] public float StepReachForce { get; private set; }

    // Methods
    public void Initialize(GameObject gameObject)
    {
        if (CapsuleCollider != null)
        {
            return;
        }

        CapsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        UpdateColliderData();
    }

    public void UpdateColliderData()
    {
        ColliderCenterInLocalSpace = CapsuleCollider.center;
    }

    public void CalculateCapsuleColliderDimensions()
    {
        SetCapsuleColliderRadius(Radius);

        SetCapsuleColliderHeight(Height * (1f - StepHeightPercentage));

        RecalculateCapsuleColliderCenter();

        float halfColliderHeight = CapsuleCollider.height / 2f;

        if (halfColliderHeight < CapsuleCollider.radius)
        {
            SetCapsuleColliderRadius(halfColliderHeight);
        }

        UpdateColliderData();
    }

    public void SetCapsuleColliderRadius(float radius)
    {
        CapsuleCollider.radius = radius;
    }
    public void SetCapsuleColliderHeight(float height)
    {
        CapsuleCollider.height = height;
    }
    public void RecalculateCapsuleColliderCenter()
    {
        float colliderHeightDifference = Height - CapsuleCollider.height;

        Vector3 newColliderCenter = new Vector3(0f, CenterY + (colliderHeightDifference / 2f), 0f);

        CapsuleCollider.center = newColliderCenter;
    }
}