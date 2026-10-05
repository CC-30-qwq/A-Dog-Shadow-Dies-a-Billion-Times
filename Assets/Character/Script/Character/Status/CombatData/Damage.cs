using System.Collections.Generic;

[System.Serializable]
public class Damage
{
    public List<MoveSpeed> moveSpeeds;
    public float SufferDuration;
    public float SufferCoolDown;
    public float ParryDuration;
    public float HitDuration;
}

[System.Serializable]
public class MoveSpeed
{
    public float moveSpeed;
}