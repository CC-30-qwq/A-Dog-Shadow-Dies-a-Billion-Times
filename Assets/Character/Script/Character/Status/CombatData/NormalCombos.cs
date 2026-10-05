using System;

[System.Serializable]
public class NormalCombos
{
    public RotateSpeed rotateSpeeds;
    [Serializable]
    public class RotateSpeed
    {
        public float NormalAttackRotateSpeed;
        public float ChargeAttackRotateSpeed;
    }
    public AttackDuration attackDurations;
    [Serializable]
    public class AttackDuration
    {
        public float NormalAttackDuration;
        public float ChargeAttackDuration;
    }
    public float ComboWindow;

    public bool CanCharge;
    public int ChargeAttackType;

    public int NextCombo;
}