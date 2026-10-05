using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class InstantiateEffect : MonoBehaviour
{
    FSM FSM;
    public GameObject effectPosition;

    private Dictionary<int, ObjectPool<GameObject>> effectPools = new Dictionary<int, ObjectPool<GameObject>>();

    void Start()
    {
        FSM = GetComponent<FSM>();

        // 初始化所有效果池
        InitPool(0, FSM.Effects.BlockFlashEffects);
        InitPool(1, FSM.Effects.BlockBigEffects);
        InitPool(2, FSM.Effects.BlockSmallEffects);
        InitPool(3, FSM.Effects.SlashEffect);
        InitPool(4, FSM.Effects.BeamEffect);
        InitPool(5, FSM.Effects.BloodEffect_0);
        InitPool(6, FSM.Effects.ArrowEffects);
    }

    void InitPool(int id, GameObject prefab)
    {
        var pool = new ObjectPool<GameObject>(
            () =>
            {
                var effect = Instantiate(prefab);
                effect.GetComponent<DestoryEffect>().pool = effectPools[id];
                effect.SetActive(false);
                return effect;
            },
            effect => effect.SetActive(true),
            effect => effect.SetActive(false),
            effect => Destroy(effect)
        );
        effectPools[id] = pool;
    }

    GameObject GetEffect(int id, Vector3 position, Quaternion rotation)
    {
        var effect = effectPools[id].Get();
        effect.transform.SetPositionAndRotation(position, rotation);
        return effect;
    }

    public void InstantiateEffect_0() => GetEffect(0, effectPosition.transform.position, FSM.transform.rotation);
    public void InstantiateEffect_1() => GetEffect(1, effectPosition.transform.position, transform.rotation);
    public void InstantiateEffect_2() => GetEffect(2, effectPosition.transform.position, transform.rotation);
    public void InstantiateEffect_3() => GetEffect(3, FSM.CombatSetting.WeaponCollider.transform.position, FSM.CombatSetting.WeaponCollider.transform.rotation);
    public void InstantiateEffect_4()
    {
        var effect = GetEffect(4, FSM.transform.position + transform.forward * -0.5f + transform.up, FSM.CombatSetting.WeaponCollider.transform.rotation);
        var rb = effect.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        effect.transform.rotation = FSM.CombatSetting.WeaponCollider.transform.rotation;
        rb.AddForce(transform.forward * 5, ForceMode.Impulse);
    }
    public void InstantiateEffect_5() => GetEffect(5, FSM.DamagePosition, Quaternion.LookRotation(FSM.DamageDirection));
    public void InstantiateEffect_6()
    {
        var effect = GetEffect(6, effectPosition.transform.position, FSM.transform.rotation);
        var rb = effect.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        effect.transform.rotation = FSM.transform.rotation;
        rb.AddForce(transform.forward * 2, ForceMode.Impulse);
    }
}
