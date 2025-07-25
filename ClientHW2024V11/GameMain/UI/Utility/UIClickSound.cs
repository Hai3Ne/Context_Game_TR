
﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SEZSJ;

public class UIClickSound : MonoBehaviour
{
    private int soundID = 1;
    public void Awake()
    {
        Button button = this.GetComponent<Button>();
        if (button)
        {
            button.onClick.AddListener(OnSoundClick);
        } 
    }
     
    public void OnSoundClick()
    {

        if (!CoreEntry.cfg_bEaxToggle)
        {
            return;
        }

        if (soundID == 0)
        {
            return;
        }

        CoreEntry.gAudioMgr.PlayUISound(soundID);
        /*        string path = AudioMgr.GetAudioPath(soundID);
                if (path == null)
                {
                    LogMgr.UnityError("无效音效ID:" + soundID);
                    return;
                }

                AudioClip clip = (AudioClip)CoreEntry.gResLoader.Load(path, typeof(AudioClip));
                if (clip == null)
                {
                    LogMgr.UnityError("音效配置错误 路径:" + path + "  id" + soundID);
                    return;
                }*/

        //    NGUITools.PlaySound(clip); 
    }
}

