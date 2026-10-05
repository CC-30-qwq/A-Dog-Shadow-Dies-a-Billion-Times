using UnityEngine;

public class AutoTip : MonoBehaviour
{
    public string tipMessage = "发现秘密！";
    public float tipDuration = 2f;
    public bool showOnce = true;  // 是否只显示一次

    private bool hasShown = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!showOnce || !hasShown)
            {
                TipManager.Instance?.ShowTip(tipMessage, tipDuration);
                hasShown = true;
            }
        }
    }
}