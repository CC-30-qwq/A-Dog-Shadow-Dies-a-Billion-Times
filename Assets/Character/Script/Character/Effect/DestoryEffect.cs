using UnityEngine;
using UnityEngine.Pool;

public class DestoryEffect : MonoBehaviour
{
    float Timer;
    public float time;
    public float attackTime;
    public ObjectPool<GameObject> pool;

    void OnEnable()
    {
        Timer = 0f; // 重置计时器，确保对象复用时从0开始计时
        if (this.gameObject.GetComponent<Collider>() != null)
        {
            this.gameObject.GetComponent<Collider>().enabled = true;
        }
    }

    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer > time)
        {
            pool.Release(this.gameObject);
        }

        if (Timer > attackTime)
        {
            if (this.gameObject.GetComponent<Collider>() != null)
            {
                this.gameObject.GetComponent<Collider>().enabled = false;
            }
        }
    }
}
