using UnityEngine;

public class InteractHide : MonoBehaviour
{
    public GameObject targetObject;  // 要隐藏的物体
    public string tipMessage = "按 E 隐藏物体";

    private bool isInRange = false;

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E))
        {
            targetObject.SetActive(false);
            TipManager.Instance?.ShowTip("已隐藏");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            TipManager.Instance?.ShowTip(tipMessage);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}