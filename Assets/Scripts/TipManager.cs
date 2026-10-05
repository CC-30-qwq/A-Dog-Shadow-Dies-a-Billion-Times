using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance;

    [Header("Tip UI")]
    public GameObject tipPanel;      // 提示面板
    public Text tipText;             // 提示文本

    [Header("Interact Hint")]
    public GameObject interactHint;  // 交互提示面板（按E）
    public Text interactText;        // 交互提示文本

    private Coroutine tipCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
          
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (tipPanel != null)
            tipPanel.SetActive(false);

        if (interactHint != null)
            interactHint.SetActive(false);
    }

    /// <summary>
    /// 显示提示消息（自动消失）
    /// </summary>
    public void ShowTip(string message, float duration = 2f)
    {
        if (tipText == null || tipPanel == null)
        {
            Debug.LogWarning("TipManager: tipText 或 tipPanel 未赋值");
            return;
        }

        if (tipCoroutine != null)
            StopCoroutine(tipCoroutine);

        tipCoroutine = StartCoroutine(DisplayTip(message, duration));
    }

    IEnumerator DisplayTip(string message, float duration)
    {
        tipText.text = message;
        tipPanel.SetActive(true);

        yield return new WaitForSeconds(duration);

        tipPanel.SetActive(false);
        tipCoroutine = null;
    }

    /// <summary>
    /// 显示/隐藏交互提示（按E键）
    /// </summary>
    public void ShowInteractHint(bool show, string objectName = "")
    {
        if (interactHint == null)
        {
            Debug.LogWarning("TipManager: interactHint 未赋值");
            return;
        }

        interactHint.SetActive(show);

        if (show && interactText != null)
        {
            if (!string.IsNullOrEmpty(objectName))
                interactText.text = "按 E 交互: " + objectName;
            else
                interactText.text = "按 E 交互";
        }
    }

    /// <summary>
    /// 隐藏交互提示
    /// </summary>
    public void HideInteractHint()
    {
        if (interactHint != null)
            interactHint.SetActive(false);
    }
}