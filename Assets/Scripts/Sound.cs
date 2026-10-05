using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolManager;
public class Sound : Singleton<Sound> {

  public   AudioSource m_Bg;
    public AudioSource m_effect;
    public string ResourcesDir = "";

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(this.gameObject);
        m_Bg = gameObject.AddComponent<AudioSource>();
        m_Bg.playOnAwake = false;
        m_Bg.loop = true;
        
        m_effect = gameObject.AddComponent<AudioSource>();

       PlayBG("雨中");
    }

    //播放背景音乐
    public void PlayBG(string audioName) {
        string oldName;
        if (m_Bg.clip == null)
        {
            oldName = "";
        }
        else {
            oldName = m_Bg.clip.name;
        }

        if (oldName != audioName) {
            //加载资源 clip
            string path = ResourcesDir + "/" + audioName;

            AudioClip clip = Resources.Load<AudioClip>(path);

            //播放
            if (clip != null) {
                m_Bg.clip = clip;
                m_Bg.Play();
            }
        }
    }

    //音效
    public void PlayEffect(string audioName) {

        string path = ResourcesDir + "/" + audioName;

        AudioClip clip = Resources.Load<AudioClip>(path);

        //播放
        m_effect.PlayOneShot(clip);
    }
}
