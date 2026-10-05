using System.Collections.Generic;
using UnityEngine;

// Cached animator hashes and common WaitForSeconds to reduce allocations and string hashing
public static class CachedAnimator
{
    public static readonly int Move = Animator.StringToHash("Move");
    public static readonly int OnAttack = Animator.StringToHash("OnAttack");

    public static readonly int Start = Animator.StringToHash("Start");
    public static readonly int Run = Animator.StringToHash("Move");
    public static readonly int Avoid = Animator.StringToHash("Avoid");
    public static readonly int LightStop = Animator.StringToHash("LightStop");
    public static readonly int HardStop = Animator.StringToHash("HardStop");
    public static readonly int Jump_Start = Animator.StringToHash("Jump_Start");
    public static readonly int Jump_Loop = Animator.StringToHash("Jump_Loop");
    public static readonly int Jump_End = Animator.StringToHash("Jump_End");
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int Block_0 = Animator.StringToHash("Block_0");
    public static readonly int Block_1 = Animator.StringToHash("Block_1");
    public static readonly int Defense = Animator.StringToHash("Defense");
    public static readonly int SprintAttack = Animator.StringToHash("SprintAttack");
    public static readonly int SkillAttack_0 = Animator.StringToHash("SPAttack_0");
    public static readonly int SkillAttack_1 = Animator.StringToHash("SPAttack_1");
}

// Cache for dynamic animation names like "Attack_0", "Charge_Type" to avoid repeated StringToHash calls
public static class CombosHashCache
{
    private static readonly Dictionary<string, int> cache = new Dictionary<string, int>();

    public static int Get(string name)
    {
        if (string.IsNullOrEmpty(name)) return 0;
        if (cache.TryGetValue(name, out int h)) return h;
        h = Animator.StringToHash(name);
        cache[name] = h;
        return h;
    }
}