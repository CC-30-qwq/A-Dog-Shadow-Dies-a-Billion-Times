using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DialogPanel : MonoBehaviour
{
    public GameObject panel;
    public Text dialogText;

    private string[] dialogLines;
    private int currentLine = 0;
    private Action onComplete;
    private bool isTyping = false;
    private string currentText = "";

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        // 鼠标点击任意位置进行对话推进
        if (panel.activeSelf && Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                // 如果正在逐字显示，直接显示完整文本
                StopAllCoroutines();
                dialogText.text = currentText;
                isTyping = false;
            }
            else
            {
                // 显示下一句
                NextLine();
            }
        }
    }

    public void ShowDialog(string[] lines, Action callback)
    {
        dialogLines = lines;
        currentLine = 0;
        onComplete = callback;

        panel.SetActive(true);
        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        currentText = dialogLines[currentLine];
        StartCoroutine(TypeText(currentText));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char c in text.ToCharArray())
        {
            dialogText.text += c;
            yield return new WaitForSeconds(0.05f); // 逐字显示间隔
        }

        isTyping = false;
    }

    void NextLine()
    {
        currentLine++;
        if (currentLine < dialogLines.Length)
        {
            // 显示下一句
            ShowCurrentLine();
        }
        else
        {
            // 对话结束，关闭面板
            CloseDialog();
        }
    }

    void CloseDialog()
    {
        panel.SetActive(false);
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}