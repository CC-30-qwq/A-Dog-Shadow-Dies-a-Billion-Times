using UnityEngine;
using UnityEngine.Events;

public class NPC : MonoBehaviour
{
    [Tooltip("对话文本")]
    public string[] DialogLines;

    [Tooltip("对话结束后触发的事件")]
    public UnityEvent OnDialogEnd;

    private bool isInRange = false;
    private DialogPanel dialogPanel;
    private bool isDialogShowing = false;

    void Start()
    {
        dialogPanel = FindObjectOfType<DialogPanel>();
    }

    void Update()
    {
        if (isInRange && !isDialogShowing && Input.GetKeyDown(KeyCode.E))
        {
            StartDialog();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            TipManager.Instance.ShowTip("按 E 对话");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    void StartDialog()
    {
        isDialogShowing = true;

        if (dialogPanel != null)
        {
            dialogPanel.ShowDialog(DialogLines, () =>
            {
                OnDialogEnd?.Invoke();
                isDialogShowing = false;
            });
        }
    }
}