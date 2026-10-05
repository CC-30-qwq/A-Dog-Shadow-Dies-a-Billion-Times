using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public Slider HP_Slider;
    public bool LookAtCamera;
    public float MaxHP = 100;
    public float HP = 100;
    public bool isPlayer = false;  // 是否是玩家

    public GameObject deathPanel;  // 死亡面板（拖入UI面板）

    private void Update()
    {
        if (HP_Slider != null)
        {
            HP_Slider.value = HP / MaxHP;
        }
        if (LookAtCamera)
        {
            HP_Slider.transform.LookAt(Camera.main.transform);
        }

        // 玩家死亡检测
        if (isPlayer && HP <= 0)
        {
            Time.timeScale = 0f;  // 暂停游戏
            deathPanel.SetActive(true);
            enabled = false;  // 停止更新
        }
    }

}
