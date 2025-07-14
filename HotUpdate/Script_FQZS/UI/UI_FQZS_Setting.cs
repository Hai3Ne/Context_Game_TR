using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FQZS_Setting : UILayer
{
    public UISlider mSliderMusic;
    public UISlider mSliderSound;

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "music":
                mSliderMusic = tf.GetComponent<UISlider>();
                break;
            case "sound":
                mSliderSound = tf.GetComponent<UISlider>();
                break;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_yes":
                Close();
                break;
        }
    }

    public void InitData()
    {
        mSliderMusic.value = AudioManager.MusicVolume;
        mSliderSound.value = AudioManager.AudioVolume;
        EventDelegate.Add(this.mSliderMusic.onChange, onMusicChange);
        EventDelegate.Add(this.mSliderSound.onChange, onAudioChange);
    }

    private void onMusicChange()
    {
        //音乐
        AudioManager.MusicVolume = this.mSliderMusic.value;
    }
    private void onAudioChange()
    {
        //音效
        AudioManager.AudioVolume = this.mSliderSound.value;
    }

    public override void OnExit()
    {
        mSliderMusic = null;
        mSliderSound = null;
    }
}
