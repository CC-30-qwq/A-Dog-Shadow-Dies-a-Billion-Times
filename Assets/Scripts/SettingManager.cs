using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (Sound.Instance) {
            bgSlider.value = Sound.Instance.m_Bg.volume;
            effctSlider.value = Sound.Instance.m_effect.volume;

        }

      
    }

    public Slider bgSlider;
    public Slider effctSlider;
    public void ChangedBg()
    {
        Sound.Instance.m_Bg.volume = bgSlider.value;
    }
    public void loadGame(int value)
    {

        SceneManager.LoadScene(value);
    }
    public void ChangedEffct()
    {
        Sound.Instance.m_effect.volume = effctSlider.value;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
